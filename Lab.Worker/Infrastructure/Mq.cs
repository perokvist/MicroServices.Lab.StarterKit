using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.v0_9_1;

namespace Lab.Worker.Infrastructure
{
    public static class MQ
    {
        public static Action<Envelope> CreatePublisherOn(IConnection connection, string exchangeName, string routingKey)
        {
            IModel channel = null;
            try
            {
                channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("Could not create model: {0}", ex.Message);
                throw;
            }
            
            return envelope =>
            {
                var jsonMessage = JsonConvert.SerializeObject(envelope.Body);
                Trace.TraceInformation("Publishing {0} to {1}", envelope.Body.GetType(), routingKey);
                channel.BasicPublish(exchangeName, routingKey, envelope.Meta, System.Text.Encoding.UTF8.GetBytes(jsonMessage));
            };
        }

        public static IDisposable StartReceivingOn(IConnection connection, string exchangeName, string routingKey, string queueName, Func<IBasicProperties, string, Task> onMessage)
        {
            var channel = connection.CreateModel();
            var name = channel.QueueDeclare(queueName, true, false, false, null).QueueName;
            channel.QueueBind(name, exchangeName, routingKey);

            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(name, false, consumer);

            var cts = new CancellationTokenSource();
            Task.Run(() => ReceiveAsync(onMessage, x => channel.BasicAck(x, false), consumer, cts.Token));

            return new DisposableAction(() =>
            {
                Trace.TraceInformation("Aborting {0}", exchangeName);
                cts.Cancel();
                channel.Abort();
            });
        }

        private static async Task ReceiveAsync(Func<IBasicProperties, string, Task> onMessage, Action<ulong> ack, QueueingBasicConsumer consumer, CancellationToken token)
        {
            Trace.TraceInformation("Listening for messages....");
            while (!token.IsCancellationRequested)
            {
                var e = (RabbitMQ.Client.Events.BasicDeliverEventArgs) consumer.Queue.Dequeue();
                var json = System.Text.Encoding.UTF8.GetString(e.Body);
                Trace.TraceInformation("Received on message {1} . Message: {0}", json, e.RoutingKey);
                await onMessage(e.BasicProperties, json); 
                Trace.TraceInformation("Ready to ack message on {0}", e.RoutingKey);
                ack(e.DeliveryTag);
                Trace.TraceInformation("Acked message on {0}", e.RoutingKey);

            }
        }
    }
}
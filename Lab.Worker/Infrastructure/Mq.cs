using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;

namespace Lab.Worker.Infrastructure
{
    public static class MQ
    {
        public static Action<T> CreatePublisherOn<T>(IConnection connection, string exchangeName, string routingKey)
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
            
            return message =>
            {
                var jsonMessage = JsonConvert.SerializeObject(message);
                Trace.TraceInformation("Publishing {0} to {1}", message.GetType(), routingKey);
                channel.BasicPublish(exchangeName, routingKey, null, System.Text.Encoding.UTF8.GetBytes(jsonMessage));
            };
        }

        public static IDisposable StartReceivingOn(IConnection connection, string exchangeName, string routingKey, string subscriptionName, Func<string, Task> onMessage)
        {
            var channel = connection.CreateModel();
            var queueName = channel.QueueDeclare(subscriptionName, true, false, false, null).QueueName;
            channel.QueueBind(queueName, exchangeName, routingKey);

            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(queueName, true, consumer);

            var cts = new CancellationTokenSource();
            Task.Run(() => ReceiveAsync(onMessage, x => channel.BasicAck(x, false), consumer, cts.Token));

            return new DisposableAction(() =>
            {
                Trace.TraceInformation("Aborting {0}", exchangeName);
                cts.Cancel();
                channel.Abort();
            });
        }

        private static async Task ReceiveAsync(Func<string, Task> onMessage, Action<ulong> ack, QueueingBasicConsumer consumer, CancellationToken token)
        {
            Trace.TraceInformation("Listening for messages....");
            while (!token.IsCancellationRequested)
            {
                var e = (RabbitMQ.Client.Events.BasicDeliverEventArgs) consumer.Queue.Dequeue();
                var json = System.Text.Encoding.UTF8.GetString(e.Body);
                Trace.TraceInformation("Received on message {1} . Message: {0}", json, e.RoutingKey);
                await onMessage(json); 
                Trace.TraceInformation("Ready to ack message on {0}", e.RoutingKey);
                ack(e.DeliveryTag);
                Trace.TraceInformation("Acked message on {0}", e.RoutingKey);

            }
        }
    }
}
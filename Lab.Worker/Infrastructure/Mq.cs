using System;
using System.Diagnostics;
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
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, "topic");

            return message =>
            {
                var jsonMessage = JsonConvert.SerializeObject(message, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                Trace.TraceInformation("Publishing {0} to {1}", message.GetType(), routingKey);
                channel.BasicPublish(exchangeName, routingKey, null, System.Text.Encoding.UTF8.GetBytes(jsonMessage));
            };
        }

        public static IDisposable StartReceivingOn(IConnection connection, string exchangeName, string routingKey, Func<byte[], Task> onMessage)
        {
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchangeName, "topic");
            var queueName = channel.QueueDeclare("receive", true, false, false, null).QueueName;
            channel.QueueBind(queueName, exchangeName, routingKey);

            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(queueName, true, consumer);

            ReceiveAsync(onMessage, x => channel.BasicAck(x, false), consumer); // Fire and forget

            return new DisposableAction(() =>
            {
                Trace.TraceInformation("Aborting {0}", exchangeName);
                channel.Abort();
            });
        }

        private static async Task ReceiveAsync(Func<byte[], Task> onMessage, Action<ulong> ack, QueueingBasicConsumer consumer)
        {
            while (true)
            {
                var e = (RabbitMQ.Client.Events.BasicDeliverEventArgs)consumer.Queue.Dequeue();
                Trace.TraceInformation("Received on {1} Message: {0}", e.Body, e.RoutingKey);
                await onMessage(e.Body);
                ack(e.DeliveryTag);
            }
        }
    }
}
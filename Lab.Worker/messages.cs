using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.v0_9_1;

namespace Lab.Worker
{
    public class Messages
    {

        public static Envelope GameCreatedEvent(String gameId, String createdBy, IEnumerable<String> players)
        {
            var body = new Dictionary<string, object>
            {
                {"createdBy", createdBy},
                {"players", players},
                {"gameType", "rock-paper-scissors"},
                {"gameUrl", "http://rps.com/games/" + gameId}
            };

            return CreateMessage(gameId, "GameCreatedEvent", body);
        }


        public static Envelope GameEndedEvent(String gameId, IDictionary<String, int> scores)
        {
            var body = new Dictionary<string, object>
            {
                {"scores", scores},
                {"result", "true"},
                {"gameType", "rock-paper-scissors"}
            };
            return CreateMessage(gameId, "GameEndedEvent", body);
        }


        public static Envelope ServiceOnlineEvent(String serviceId, String description, String createdBy, String serviceUrl, string sourceUrl)
        {
            var body = new Dictionary<string, object>
            {
                {"description", description},
                {"createdBy", createdBy},
                {"serviceUrl", serviceUrl},
                {"sourceUrl", sourceUrl}
            };
            return CreateMessage(serviceId, "ServiceOnlineEvent", body);
        }

        public static Envelope LogEvent(String serviceId, LogLevel level, String context, String message)
        {
            var body = new Dictionary<string, object> { { "level", level.ToString() }, { "context", context }, { "message", message } };
            return CreateMessage(serviceId, "LogEvent", body);
        }

        public static Envelope ServiceOfflineEvent(String serviceId)
        {
            var body = new Dictionary<string, object>();
            return CreateMessage(serviceId, "ServiceOfflineEvent", body);
        }

        public enum LogLevel
        {
            TRACE,
            DEBUG,
            INFO,
            WARN,
            ERROR
        }

        private static Envelope CreateMessage(String streamId, String type, IDictionary<String, Object> body)
        {
            var meta = new BasicProperties
            {
                AppId = Assembly.GetExecutingAssembly().GetName().Name,
                Type = type,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = new AmqpTimestamp(DateTime.UtcNow.Ticks),
                Headers = new ListDictionary {{"streamId", streamId}}
            };

            return new Envelope() { Body = body, Meta = meta };
        }
    }
    public class Envelope
    {
        public object Body { get; set; }
        public IBasicProperties Meta { get; set; }
    }
}
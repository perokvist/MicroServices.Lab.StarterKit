using System;
using System.Collections.Generic;

namespace Lab.Worker
{
    public class Messages
    {

        public static Object GameCreatedEvent(String gameId, String createdBy, IEnumerable<String> players)
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


        public static Object GameEndedEvent(String gameId, IDictionary<String, int> scores) {
            var body = new Dictionary<string, object>
            {
                {"scores", scores},
                {"result", "true"},
                {"gameType", "rock-paper-scissors"}
            };
            return CreateMessage(gameId, "GameEndedEvent", body);
        }


        public static Object ServiceOnlineEvent(String serviceId, String name, String entryPoint, String createdBy) {
            var body = new Dictionary<string, object>
            {
                {"name", name},
                {"createdBy", createdBy},
                {"entryPoint", entryPoint}
            };
            return CreateMessage(serviceId, "ServiceOnlineEvent", body);
        }

        public static Object LogEvent(String serviceId, LogLevel level, String context, String message) {
            var body = new Dictionary<string,object> {{"level", level.ToString()}, {"context", context}, {"message", message}};
            return CreateMessage(serviceId, "LogEvent", body);
        }

        public static Object ServiceOfflineEvent(String serviceId)  {
            var body = new Dictionary<string, object>();
            return CreateMessage(serviceId, "ServiceOfflineEvent", body);
        }


        private static Object CreateMessage(String streamId, String type, IDictionary<String, Object> body) {
            var meta = new Dictionary<string, object>();
            var message = new Dictionary<String, Object>
            {
                {"type", type},
                {"body", body},
                {"streamId", streamId},
                {"createdAt", DateTime.UtcNow.Ticks},
                {"meta", meta}
            };
            return message;
        }
    }
}
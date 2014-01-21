using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab.Worker.Messages
{
    public class GameCreatedEvent
    {
        public GameCreatedEvent(string streamId, string createdBy, IEnumerable<string> players , string gameType, Uri gameUrl) 
        {
            StreamId = streamId;
            CreatedBy = createdBy;
            Players = players.ToList();
            GameType = gameType;
            GameUrl = gameUrl;
        }

        public string StreamId { get; private set; }
        public string CreatedBy { get; private set; }
        public IEnumerable<string> Players { get; private set; }
        public string GameType { get; private set; }
        public Uri GameUrl { get; private set; }
    }
}
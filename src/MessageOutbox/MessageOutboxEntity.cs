using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MessageOutbox
{
    public class MessageOutboxEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string MessageId { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Modified { get; set; }

        public bool IsProcessed { get; set; }

        public string MessageContent { get; set; }
    }
}

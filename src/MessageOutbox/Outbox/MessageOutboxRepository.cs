using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageOutbox.Outbox
{
    public interface IMessageOutboxRepository
    {
        Task Save(IMessage message);

        Task Update(IMessage message, bool isProcessed);

        Task<IList<IMessage>> GetUnprocessed();

        Task ExecuteTransaction(Func<Task> action);
    }

    public class MessageOutboxRepository : IMessageOutboxRepository
    {
        private readonly IMongoCollection<MessageOutboxEntity> messages;
        private readonly MongoClient client;

        public MessageOutboxRepository(IOptionsMonitor<MessageOutboxSettings> messageOutboxSettings)
        {
            var settings = messageOutboxSettings.CurrentValue;
            client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.DatabaseName);

            messages = CreateCollectionIfNotExists(database, settings.CollectionName);
        }

        public async Task<IList<IMessage>> GetUnprocessed()
        {
            var allMessages = await messages.Find(message => !message.IsProcessed).ToListAsync();
            return allMessages
                .Select(message => new Message(message.MessageId))
                .Cast<IMessage>()
                .ToList();
        }

        public async Task Update(IMessage message, bool isProcessed)
        {
            var messageEntity = Get(message.Id);
            messageEntity.IsProcessed = isProcessed;
            messageEntity.Modified = DateTimeOffset.UtcNow;
            await messages.ReplaceOneAsync(messageEntity => messageEntity.MessageId == message.Id, messageEntity);
        }

        public async Task Save(IMessage message)
        {
            var messageEntity = new MessageOutboxEntity()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                MessageId = message.Id,
                Created = DateTimeOffset.UtcNow,
                IsProcessed = false,
                MessageContent = JsonConvert.SerializeObject(message)
            };

            await messages.InsertOneAsync(messageEntity);
        }

        public async Task ExecuteTransaction(Func<Task> action)
        {
            using var session = await client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                await action();
                await session.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await session.AbortTransactionAsync();
            }
        }

        private MessageOutboxEntity Get(string id) => messages
            .Find(message => message.MessageId == id)
            .FirstOrDefault();

        private IMongoCollection<MessageOutboxEntity> CreateCollectionIfNotExists(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var options = new ListCollectionNamesOptions { Filter = filter };

            if (!database.ListCollectionNames(options).Any())
            {
                database.CreateCollection(collectionName);
            }

            return database.GetCollection<MessageOutboxEntity>(collectionName);
        }
    }
}

using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageOutbox
{
    public interface IMessageOutboxRepository
    {
        Task Save();

        IList<IMessage> Get();
    }

    internal class MessageOutboxRepository : IMessageOutboxRepository
    {
        private readonly IMongoCollection<Message> messages;

        public IList<IMessage> Get()
        {
            throw new System.NotImplementedException();
        }

        public Task Save()
        {
            throw new System.NotImplementedException();
        }
    }
}


using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageOutbox
{
    internal class MessageOutbox : IMessageOutbox
    {
        public IList<IMessage> Get()
        {
            throw new System.NotImplementedException();
        }

        public Task Save()
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IMessageOutbox
    { 
        Task Save();
        
        IList<IMessage> Get();
    }
}

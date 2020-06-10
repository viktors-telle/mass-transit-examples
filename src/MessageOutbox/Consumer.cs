using MassTransit;
using System;
using System.Threading.Tasks;

namespace MessageOutbox
{
    internal class Consumer : IConsumer<IMessage>
    {
        public Task Consume(ConsumeContext<IMessage> context)
        {
            throw new NotImplementedException();
        }
    }
}

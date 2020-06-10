using MassTransit;
using System;
using System.Threading.Tasks;

namespace MessageOutbox
{
    internal class Consumer : IConsumer<IMessage>
    {
        public Task Consume(ConsumeContext<IMessage> context)
        {
            Console.WriteLine($"Message with ID \"{context.Message.Id}\" consumed.");
            return Task.CompletedTask;
        }
    }
}

using MassTransit;
using System;
using System.Threading.Tasks;

namespace MessageOutbox
{
    public class Consumer : IConsumer<IMessage>
    {
        public async Task Consume(ConsumeContext<IMessage> context)
        {
            await Console.Out.WriteLineAsync($"Message with ID \"{context.Message.Id}\" consumed.");
        }
    }
}

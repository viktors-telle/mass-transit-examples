using MassTransit;
using MessageOutbox.Outbox;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MessageOutbox.Consumer
{
    public class MessageConsumer : IConsumer<IMessage>
    {
        private readonly ILogger<MessageConsumer> logger;

        public MessageConsumer(ILogger<MessageConsumer> logger)
        {
            this.logger = logger;
        }
        public Task Consume(ConsumeContext<IMessage> context)
        {
            logger.LogInformation($"Message with ID \"{context.Message.Id}\" consumed.");
            return Task.CompletedTask;
        }
    }
}

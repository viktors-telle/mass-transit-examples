using MessageOutbox;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MessageOutboxProcessingJob
{
    internal class MessageOutboxProcessor : IMessageOutboxProcessor
    {
        private readonly ILogger<MessageOutboxProcessor> logger;

        public MessageOutboxProcessor(ILogger<MessageOutboxProcessor> logger)
        {
            this.logger = logger;
        }

        public Task ProcessFailedMessage(IMessage message)
        {
            logger.LogInformation($"Message with ID \"{message.Id}\" processed");
            return Task.CompletedTask;
        }
    }
}

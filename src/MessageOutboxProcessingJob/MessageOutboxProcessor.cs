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

            // TODO: Create DB transaction.

            // TODO: Fetch failed messages from DB.

            // TODO: Resend them to the queue.

            // TODO: Change message the status.

            // TODO: Commit transaction.

            return Task.CompletedTask;
        }
    }
}

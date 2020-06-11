using MassTransit;
using MessageOutbox;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MessageOutboxProcessingJob
{
    internal class MessageOutboxProcessor : IMessageOutboxProcessor
    {
        private readonly IMessageOutboxRepository messageOutboxRepository;
        private readonly IBusControl bus;
        private readonly ILogger<MessageOutboxProcessor> logger;

        public MessageOutboxProcessor(
            IMessageOutboxRepository messageOutboxRepository,
            IBusControl bus,
            ILogger<MessageOutboxProcessor> logger
            )
        {
            this.messageOutboxRepository = messageOutboxRepository;
            this.bus = bus;
            this.logger = logger;
        }

        public async Task ProcessFailedMessages()
        {
            await messageOutboxRepository.ExecuteTransaction(async () =>
            {
                var unprocessedMessages = await messageOutboxRepository.GetUnprocessed();
               
                var unprocessedMessageTasks = unprocessedMessages
                    .Select(unprocessedMessage => ProcessFailedMessage(unprocessedMessage));

                await Task.WhenAll(unprocessedMessageTasks);
            });
        }

        private async Task ProcessFailedMessage(IMessage message)
        {
            logger.LogInformation($"Processing message with ID {message.Id}.");

            await bus.Publish(message);
            await messageOutboxRepository.Update(message);

            logger.LogInformation($"Finished processing message with ID {message.Id}.");
        }
    }
}

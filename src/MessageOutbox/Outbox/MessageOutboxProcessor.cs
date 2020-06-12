using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MessageOutbox.Outbox
{
    internal interface IMessageOutboxProcessor
    {
        Task ProcessFailedMessages();
    }

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

            try
            {
                await bus.Publish(message);
                await messageOutboxRepository.Update(message, true);
            }
            catch (Exception ex)
            {
                await messageOutboxRepository.Update(message, false);
                logger.LogWarning($"Message processing with ID {message.Id} failed. " +
                    $"{Environment.NewLine} Exception: {ex}");
            }


            logger.LogInformation($"Finished processing message with ID {message.Id}.");
        }
    }
}

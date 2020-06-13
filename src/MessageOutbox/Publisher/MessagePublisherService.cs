using MassTransit;
using MessageOutbox.Outbox;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageOutbox.Publisher
{
    public class MessagePublisherService : IHostedService
    {
        private const int PublishedMessageCount = 16;
        private readonly IBusControl bus;
        private readonly IMessageOutboxRepository messageOutboxRepository;
        private readonly ILogger<MessagePublisherService> logger;

        public MessagePublisherService(
            IBusControl bus,
            IMessageOutboxRepository messageOutboxRepository,
            ILogger<MessagePublisherService> logger
            )
        {
            this.bus = bus;
            this.messageOutboxRepository = messageOutboxRepository;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (var publishedMessage = 0; publishedMessage < PublishedMessageCount; publishedMessage++)
            {
                var message = new Message(Guid.NewGuid().ToString());
                try
                {
                     await bus.Publish(message);
                }
                catch (Exception)
                {
                    await messageOutboxRepository.Save(message);
                    logger.LogWarning($"Message with ID {message.Id} publishing failed, " +
                        $"and it was saved in database for later processing.");
                }                              
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

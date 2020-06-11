using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageOutbox
{
    public class MessagePublisher : IHostedService
    {
        private const int PublishedMessageCount = 5;
        private readonly IMessageOutboxRepository messageOutboxRepository;

        public MessagePublisher(IMessageOutboxRepository messageOutboxRepository)
        {
            this.messageOutboxRepository = messageOutboxRepository;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (var publishedMessage = 0; publishedMessage < PublishedMessageCount; publishedMessage++)
            {
                await messageOutboxRepository.Save(new Message(Guid.NewGuid().ToString()));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

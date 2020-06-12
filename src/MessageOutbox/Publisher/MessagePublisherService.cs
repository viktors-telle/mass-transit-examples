using MessageOutbox.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageOutbox.Publisher
{
    public class MessagePublisherService : IHostedService
    {
        private const int PublishedMessageCount = 16;
        private readonly IServiceProvider services;

        public MessagePublisherService(IServiceProvider services)
        {
            this.services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();
            var messageOutboxRepository = scope.ServiceProvider.GetRequiredService<IMessageOutboxRepository>();
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

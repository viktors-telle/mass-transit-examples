using System;
using System.Threading;
using System.Threading.Tasks;
using MessageOutbox.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessageOutbox.OutboxProcessor
{
    internal class MessageOutboxProcessorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider services;
        private readonly ILogger<MessageOutboxProcessorBackgroundService> logger;

        public MessageOutboxProcessorBackgroundService(IServiceProvider services, ILogger<MessageOutboxProcessorBackgroundService> logger)
        {
            this.services = services;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = services.CreateScope();
            var messageOutboxProcessor = scope.ServiceProvider.GetRequiredService<IMessageOutboxProcessor>();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await messageOutboxProcessor.ProcessFailedMessages();
                await Task.Delay((int)TimeSpan.FromSeconds(5).TotalMilliseconds, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");
            await Task.CompletedTask;
        }
    }
}

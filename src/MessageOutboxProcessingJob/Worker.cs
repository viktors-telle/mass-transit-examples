using System;
using System.Threading;
using System.Threading.Tasks;
using MessageOutbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessageOutboxProcessingJob
{
    internal class Worker : BackgroundService
    {
        private readonly IServiceProvider services;
        private readonly ILogger<Worker> logger;

        public Worker(IServiceProvider services, ILogger<Worker> logger)
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
                await messageOutboxProcessor.ProcessFailedMessage(new Message(Guid.NewGuid().ToString()));
                await Task.Delay((int)TimeSpan.FromSeconds(5).TotalMilliseconds, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");

            await Task.CompletedTask;
        }
    }
}

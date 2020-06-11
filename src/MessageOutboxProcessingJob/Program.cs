using GreenPipes;
using MassTransit;
using MessageOutbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace MessageOutboxProcessingJob
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<IMessageOutboxProcessor, MessageOutboxProcessor>();
                    services.AddScoped<IMessageOutboxRepository, MessageOutboxRepository>();
                    services.AddHostedService<Worker>();
                    services.Configure<MessageOutboxSettings>(hostContext.Configuration.GetSection("MessageOutboxSettings"));
                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddBus(CreateBusControl);
                    });
                });

        private static IBusControl CreateBusControl(IRegistrationContext<IServiceProvider> serviceProvider)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost");
                cfg.UseDelayedExchangeMessageScheduler();

                cfg.ReceiveEndpoint("message-queue", e =>
                {
                    e.UseScheduledRedelivery(retryConfigurator =>
                        {
                            retryConfigurator.Intervals(
                                TimeSpan.FromMinutes(1),
                                TimeSpan.FromMinutes(2),
                                TimeSpan.FromMinutes(3)
                            );
                        }
                    );

                    e.UseMessageRetry(retryConfigurator =>
                        {
                            retryConfigurator.Incremental(
                                3,
                                TimeSpan.FromSeconds(1),
                                TimeSpan.FromSeconds(15)
                            );
                        }
                    );
                });                
            });
        }
    }
}

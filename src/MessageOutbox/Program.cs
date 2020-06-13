using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using MessageOutbox.Consumer;
using MessageOutbox.Outbox;
using MessageOutbox.OutboxProcessor;
using MessageOutbox.Publisher;

namespace MessageOutbox
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
                    services.Configure<MessageOutboxSettings>(hostContext.Configuration.GetSection("MessageOutboxSettings"));
                    
                    services.AddSingleton<IMessageOutboxProcessor, MessageOutboxProcessor>();                    
                    services.AddSingleton<IMessageOutboxRepository, MessageOutboxRepository>();
                    services.AddSingleton<MessageOutboxSettings>();

                    services.AddHostedService<MessageOutboxProcessorBackgroundService>();
                    services.AddHostedService<MessagePublisherService>();
                    services.AddHostedService<ConsumerHostedService>();

                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddConsumer<MessageConsumer>();
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
                    e.PrefetchCount = 16;
                    e.Consumer<MessageConsumer>(serviceProvider.Container);

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

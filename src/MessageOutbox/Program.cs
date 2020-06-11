using MassTransit;
using System;
using GreenPipes;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageOutbox
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appSettings.json", optional: true);
                    config.AddEnvironmentVariables();

                    if (args != null) config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<MessageOutboxSettings>(hostContext.Configuration.GetSection("MessageOutboxSettings"));

                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddBus(CreateBusControl);
                        cfg.AddConsumer<Consumer>();
                    });

                    services.AddScoped<IMessageOutboxRepository, MessageOutboxRepository>();
                    services.AddSingleton<MessageOutboxSettings>();
                    services.AddHostedService<MassTransitHostedService>();
                    services.AddHostedService<MessagePublisher>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }

        private static IBusControl CreateBusControl(IRegistrationContext<IServiceProvider> serviceProvider)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost");
                cfg.UseDelayedExchangeMessageScheduler();

                cfg.ReceiveEndpoint("message-queue", e =>
                {
                    e.Consumer<Consumer>(serviceProvider.Container);

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

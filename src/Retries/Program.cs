using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;

namespace Retries
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            var busControl = CreateBusControl();
            await StartBusControl(busControl);
        }

        private static IBusControl CreateBusControl()
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost");
                // Enable redelivery.
                cfg.UseDelayedExchangeMessageScheduler();

                cfg.ReceiveEndpoint("message-queue", e =>
                {
                    // Configure redelivery retries.
                    e.UseScheduledRedelivery(retryConfigurator =>
                        {
                            // Do not retry "NameTooShortException" exception.
                            retryConfigurator.Ignore(typeof(NameTooShortException));

                            retryConfigurator.Intervals(
                                TimeSpan.FromMinutes(1),
                                TimeSpan.FromMinutes(2),
                                TimeSpan.FromMinutes(3)
                            );
                        }
                    );

                    e.UseMessageRetry(retryConfigurator =>
                        {
                            // Do not retry "NameTooShortException" exception.
                            retryConfigurator.Ignore(typeof(NameTooShortException));

                            retryConfigurator.Incremental(
                                3,
                                TimeSpan.FromSeconds(1),
                                TimeSpan.FromSeconds(15)
                            );
                        }
                    );

                    e.Consumer<Consumer>();
                });
            });
        }

        private static async Task StartBusControl(IBusControl busControl)
        {
            await busControl.StartAsync();
            var messagePublished = false;
            try
            {
                do
                {
                    if (messagePublished) { continue; }

                    await busControl.Publish<IMessage>(
                        new Message(Guid.NewGuid().ToString(), "Valid name")
                    );

                    await busControl.Publish<IMessage>(
                        new Message(Guid.NewGuid().ToString(), "Short")
                    );

                    messagePublished = true;

                } while (true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while publishing the message: {e}");
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
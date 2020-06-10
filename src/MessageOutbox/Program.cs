using MassTransit;
using System;
using GreenPipes;
using System.Threading.Tasks;

namespace MessageOutbox
{
    internal static class Program
    {
        public static async Task Main(string[] args)
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

                    e.Consumer<Consumer>();
                });
            });
        }

        private static async Task StartBusControl(IBusControl busControl)
        {
            await busControl.StartAsync();
            try
            {
                await busControl.Publish<IMessage>(new Message(Guid.NewGuid().ToString()));
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

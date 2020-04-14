using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;

namespace MassTransitExamples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost");
                cfg.UseDelayedExchangeMessageScheduler();

                cfg.ReceiveEndpoint("message-queue", e =>
                {
                    e.UseScheduledRedelivery(retryConfigurator =>
                        retryConfigurator.Intervals(
                            TimeSpan.FromMinutes(1),
                            TimeSpan.FromMinutes(2),
                            TimeSpan.FromMinutes(3)
                        )
                    );

                    e.UseMessageRetry(retryConfigurator =>
                        retryConfigurator.Incremental(
                            3,
                            TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(15)
                        )
                    );

                    e.Consumer<Consumer>();
                });
            });

            await busControl.StartAsync();
            try
            {
                do
                {
                    await busControl.Publish<IMessage>(new Message
                    {
                        Id = Guid.NewGuid().ToString()
                    });

                    await Task.Delay((int) TimeSpan.FromMinutes(5).TotalMilliseconds);

                } while (true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while publishing the message: {e.ToString()}");
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
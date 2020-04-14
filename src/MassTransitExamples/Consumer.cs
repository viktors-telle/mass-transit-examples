using System;
using System.Threading.Tasks;
using MassTransit;

namespace MassTransitExamples
{
    public class Consumer : IConsumer<IMessage>
    {
        public Task Consume(ConsumeContext<IMessage> context)
        {
            Console.WriteLine($"Message with ID {context.Message.Id} consumed.");

            throw new Exception("Unable to connect to external service.");
        }
    }

    public interface IMessage
    {
        string Id { get; }
    }

    public class Message
    {
        public string Id { get; set; }
    }
}
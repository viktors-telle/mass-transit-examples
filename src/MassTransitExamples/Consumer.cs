using System;
using System.Threading.Tasks;
using MassTransit;

namespace MassTransitExamples
{
    public class Consumer : IConsumer<IMessage>
    {
        public Task Consume(ConsumeContext<IMessage> context)
        {
            var message = context.Message;
            Console.WriteLine($"Message with ID \"{message.Id}\" and Name \"{message.Name}\" consumed.");

            if (message.Name.Length <= 5)
            {
                throw new NameTooShortException($"Message name is {message.Name.Length} long, but it is expected to be at least 6 characters long.");
            }

            throw new ExternalServiceUnavailableException("Unable to connect to external service.");
        }
    }
}
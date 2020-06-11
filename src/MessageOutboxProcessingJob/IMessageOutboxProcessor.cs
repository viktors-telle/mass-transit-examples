using System.Threading.Tasks;

namespace MessageOutboxProcessingJob
{
    internal interface IMessageOutboxProcessor
    {
        Task ProcessFailedMessages();
    }
}
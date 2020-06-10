using MessageOutbox;
using System.Threading.Tasks;

namespace MessageOutboxProcessingJob
{
    internal interface IMessageOutboxProcessor
    {
        Task ProcessFailedMessage(IMessage message);
    }
}
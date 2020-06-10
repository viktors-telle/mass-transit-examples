namespace Retries
{
    public interface IMessage
    {
        string Id { get; }

        string Name { get; }
    }
}
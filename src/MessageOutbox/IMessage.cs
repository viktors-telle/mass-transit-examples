namespace MessageOutbox
{
    public interface IMessage
    {
        string Id { get; }
    }
}
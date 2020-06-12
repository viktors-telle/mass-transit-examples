namespace MessageOutbox.Outbox
{
    public interface IMessage
    {
        string Id { get; }
    }

    public class Message : IMessage
    {
        public Message(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
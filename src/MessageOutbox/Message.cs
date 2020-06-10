namespace MessageOutbox
{
    public class Message : IMessage
    {
        public Message(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
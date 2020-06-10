namespace MessageOutbox
{
    internal class Message
    {
        public Message(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
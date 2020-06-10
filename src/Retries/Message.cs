namespace Retries
{
    internal class Message : IMessage
    {
        public Message(string id, string name)
        {
            Id = id;
            Name = name;    
        }

        public string Id { get; }

        public string Name { get; }
    }
}
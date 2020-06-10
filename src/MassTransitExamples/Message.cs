namespace Retries
{
    public class Message
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
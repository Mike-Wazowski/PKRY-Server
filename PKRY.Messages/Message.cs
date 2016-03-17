namespace PKRY.Messages
{
    public class Message
    {
        public string Content
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public Message() { }
        public Message(string username, string content)
        {
            Content = content;
            Username = username;
        }

        public static Message Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<Message>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
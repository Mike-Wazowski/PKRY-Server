namespace PKRY.Messages
{
    public class RBigInt
    {
        public string Username
        {
            get;
            set;
        }
        public string Number
        {
            get;
            set;
        }

        public RBigInt() { }
        public RBigInt(string username, string number)
        {
            Username = username;
            Number = number;
        }

        public static RBigInt Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<RBigInt>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
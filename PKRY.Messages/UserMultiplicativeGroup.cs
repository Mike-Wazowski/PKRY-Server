namespace PKRY.Messages
{
    public class UserMultiplicativeGroup
    {
        public string Username
        {
            get;
            set;
        }
        public string P
        {
            get;
            set;
        }
        public string G
        {
            get;
            set;
        }
        public string A
        {
            get;
            set;
        }

        public UserMultiplicativeGroup() { }
        public UserMultiplicativeGroup(string username, string p, string g)
        {
            Username = username;
            P = p;
            G = g;
        }

        public static UserMultiplicativeGroup Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<UserMultiplicativeGroup>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
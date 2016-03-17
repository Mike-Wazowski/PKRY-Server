namespace PKRY.Messages
{
    public class Login
    {
        public string Username
        {
            get;
            set;
        }
        public string Password
        {
            get;
            set;
        }

        public Login() { }
        public Login(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public static Login Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<Login>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
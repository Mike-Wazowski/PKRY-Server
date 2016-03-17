namespace PKRY.Messages
{
    public class RegisterResponse
    {
        public string AESKey
        {
            get;
            set;
        }

        public RegisterResponse() { }
        public RegisterResponse(string AESKey)
        {
            this.AESKey = AESKey;
        }

        public static RegisterResponse Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<RegisterResponse>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
namespace PKRY.Messages
{
    public class Register
    {
        public string Modulus
        {
            get;
            set;
        }
        public string Exponent
        {
            get;
            set;
        }

        public Register() { }
        public Register(string modulus, string exponent)
        {
            Modulus = modulus;
            Exponent = exponent;
        }

        public static Register Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<Register>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
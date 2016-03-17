using Newtonsoft.Json;
using System;

namespace PKRY.Messages
{
    public class DHBigInt
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

        public DHBigInt() { }
        public DHBigInt(string username, string number)
        {
            Username = username;
            Number = number;
        }

        public static DHBigInt Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<DHBigInt>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
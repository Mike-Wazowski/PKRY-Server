using System.Collections.Generic;

namespace PKRY.Messages
{
    public class LoginResponse
    {
        public bool IsCorrect
        {
            get;
            set;
        }
        public List<string> Usernames
        {
            get;
            set;
        }

        public LoginResponse() { }
        public LoginResponse(bool isCorrect, List<string> usernames)
        {
            IsCorrect = isCorrect;
            if (usernames != null)
                Usernames = usernames;
            else
                Usernames = new List<string>();
        }

        public static LoginResponse Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<LoginResponse>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
using Newtonsoft.Json;
using System;

namespace PKRY.Messages
{
    public class InviteResponse
    {
        public bool IsAccepted
        {
            get;
            set;
        }
        public UserMultiplicativeGroup UsersMultiplicativeGroup
        {
            get;
            set;
        }

        public InviteResponse() { }
        public InviteResponse(bool isAccepted, UserMultiplicativeGroup userMultiplicativeGroup)
        {
            IsAccepted = isAccepted;
            UsersMultiplicativeGroup = userMultiplicativeGroup;
        }

        public static InviteResponse Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<InviteResponse>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
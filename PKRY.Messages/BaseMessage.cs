using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKRY.Messages
{
    public enum MessageType
    {
        Register,//
        RegisterResponse,//
        Login,//
        LoginResponse,//
        StatusNotifier,//
        Logout,//
        InviteResponse,//
        GroupCancelled,//
        MultiplicativeGroupNotifier,//
        DHBigInt,//
        RBigInt,//
        Message,//
        LeaveGroup,//
        LeaveGroupNotifier//
    };

    public class BaseMessage
    {
        public MessageType Type
        {
            get;
            set;
        }
        public string SerializedMessage
        {
            get;
            set;
        }

        public BaseMessage() { }
        public BaseMessage(MessageType type, string serializedMessage)
        {
            Type = type;
            SerializedMessage = serializedMessage;
        }

        public static BaseMessage Deserialize(string serializedMessage)
        {
            var deserializedMessage = JsonConvert.DeserializeObject<BaseMessage>(serializedMessage);
            return deserializedMessage;
        }
    }
}

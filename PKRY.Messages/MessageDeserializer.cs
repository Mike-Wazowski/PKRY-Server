using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PKRY.Messages
{
    public class MessageDeserializer<T>
    {
        public T Deserialize(BaseMessage baseMessage)
        {
            if (String.IsNullOrEmpty(baseMessage.SerializedMessage))
                throw new ArgumentException("SerializedMessage can not be null");
            object deserializedMessage = null;
            switch(baseMessage.Type)
            {
                case MessageType.DHBigInt:
                    deserializedMessage = JsonConvert.DeserializeObject<DHBigInt>(baseMessage.SerializedMessage);
                    break;
                case MessageType.InviteResponse:
                    deserializedMessage = JsonConvert.DeserializeObject<InviteResponse>(baseMessage.SerializedMessage);
                    break;
                case MessageType.LeaveGroupNotifier:
                    deserializedMessage = JsonConvert.DeserializeObject<LeaveGroupNotifier>(baseMessage.SerializedMessage);
                    break;
                case MessageType.Login:
                    deserializedMessage = JsonConvert.DeserializeObject<Login>(baseMessage.SerializedMessage);
                    break;
                case MessageType.LoginResponse:
                    deserializedMessage = JsonConvert.DeserializeObject<LoginResponse>(baseMessage.SerializedMessage);
                    break;
                case MessageType.Message:
                    deserializedMessage = JsonConvert.DeserializeObject<Message>(baseMessage.SerializedMessage);
                    break;
                case MessageType.MultiplicativeGroupNotifier:
                    deserializedMessage = JsonConvert.DeserializeObject<MultiplicativeGroupNotifier>(baseMessage.SerializedMessage);
                    break;
                case MessageType.RBigInt:
                    deserializedMessage = JsonConvert.DeserializeObject<RBigInt>(baseMessage.SerializedMessage);
                    break;
                case MessageType.Register:
                    deserializedMessage = JsonConvert.DeserializeObject<Register>(baseMessage.SerializedMessage);
                    break;
                case MessageType.RegisterResponse:
                    deserializedMessage = JsonConvert.DeserializeObject<RegisterResponse>(baseMessage.SerializedMessage);
                    break;
                case MessageType.StatusNotifier:
                    deserializedMessage = JsonConvert.DeserializeObject<StatusNotifier>(baseMessage.SerializedMessage);
                    break;
            }
            return (T)deserializedMessage;
        }
    }
}

using System.Collections.Generic;

namespace PKRY.Messages
{
    public class StatusNotifier
    {
        public bool IsGroupInvite
        {
            get;
            set;
        }
        public List<string> Usernames
        {
            get;
            set;
        }
        public int GroupSize
        {
            get;
            set;
        }

        public StatusNotifier() { }
        public StatusNotifier(bool isGroupInvite, List<string> usernames)
        {
            IsGroupInvite = isGroupInvite;
            if (usernames != null)
                Usernames = usernames;
            else
                Usernames = new List<string>();
        }

        public static StatusNotifier Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<StatusNotifier>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
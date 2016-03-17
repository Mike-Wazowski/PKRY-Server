namespace PKRY.Messages
{
    public class LeaveGroupNotifier
    {
        public string Username
        {
            get;
            set;
        }

        public LeaveGroupNotifier() { }
        public LeaveGroupNotifier(string username)
        {
            Username = username;
        }

        public static LeaveGroupNotifier Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<LeaveGroupNotifier>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
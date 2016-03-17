namespace PKRY.Messages
{
    public class MultiplicativeGroupNotifier
    {
        public UserMultiplicativeGroup UserMultiplicativeGroup
        {
            get;
            set;
        }

        public MultiplicativeGroupNotifier() { }
        public MultiplicativeGroupNotifier(UserMultiplicativeGroup userMultiplicativeGroup)
        {
            UserMultiplicativeGroup = userMultiplicativeGroup;
        }

        public static MultiplicativeGroupNotifier Deserialize(BaseMessage baseMessage)
        {
            var deserializer = new MessageDeserializer<MultiplicativeGroupNotifier>();
            return deserializer.Deserialize(baseMessage);
        }
    }
}
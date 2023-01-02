namespace PubSubDemo.Message
{
    public class GenericMessage<T> : IMessage
    {
        public T Value { get; init; }

        public GenericMessage(T value) => Value = value;
    }
}
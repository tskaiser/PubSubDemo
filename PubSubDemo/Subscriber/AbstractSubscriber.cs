using PubSubDemo.Message;

namespace PubSubDemo.Subscriber
{
    public abstract class AbstractSubscriber<T> : IMessageSubscriber
        where T : IMessage
    {
        public abstract void Receive(string channelName, T message);

        public void Receive(string channelName, IMessage message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            var castedMessage = (T)message;
            if (castedMessage is null)
                throw new ArgumentException(
                    $"Expected to receive a message of type '${typeof(T)}'," +
                    $" instead got a message of type '${message.GetType()}'");

            Receive(channelName, castedMessage);
        }
    }
}
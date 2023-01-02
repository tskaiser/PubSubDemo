using PubSubDemo.Message;

namespace PubSubDemo.Subscriber
{
    public interface IMessageSubscriber
    {
        public void Receive(string channelName, IMessage message);
    }
}
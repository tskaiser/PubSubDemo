using PubSubDemo.Channel;
using PubSubDemo.Message;
using PubSubDemo.Subscriber;

namespace PubSubDemo.Broker
{
    public interface IMessageBroker
    {
        public void Publish<T>(string channelName, T message)
            where T : IMessage;

        public IChannel CreateChannel(string channelName);

        public IChannel GetChannel(string channelName);

        public void RemoveChannel(string channelName);

        public void Subscribe<T>(string channelName, AbstractSubscriber<T> subscriber)
            where T : IMessage;

        public void Unsubscribe<T>(string channelName, AbstractSubscriber<T> subscriber)
            where T : IMessage;
    }
}
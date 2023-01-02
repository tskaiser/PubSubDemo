using PubSubDemo.Channel;
using PubSubDemo.Message;
using PubSubDemo.Subscriber;

namespace PubSubDemo.Broker
{
    public class DemoBroker : IMessageBroker
    {
        private readonly Dictionary<string, IChannel> _channels;

        public DemoBroker() => _channels = new Dictionary<string, IChannel>();

        public IChannel CreateChannel(string channelName)
        {
            if (_channels.TryGetValue(channelName, out _))
                throw new ArgumentException(
                    $"There already exist a channel named '${channelName}' registered with this broker.");

            var channel = new DemoChannel(channelName);
            _channels[channelName] = channel;

            return channel;
        }

        public IChannel GetChannel(string channelName) =>
            GetOrThrow(channelName);

        public void Publish<T>(string channelName, T message)
            where T : IMessage =>
            GetOrThrow(channelName).Publish(message);

        public void RemoveChannel(string channelName) =>
            _channels.Remove(channelName);

        public void Subscribe<T>(string channelName, AbstractSubscriber<T> subscriber)
            where T : IMessage =>
            GetOrThrow(channelName).Subscribe(subscriber);

        public void Unsubscribe<T>(string channelName, AbstractSubscriber<T> subscriber)
            where T : IMessage =>
            GetOrThrow(channelName).Unsubscribe(subscriber);

        private IChannel GetOrThrow(string channelName) =>
            _channels.TryGetValue(channelName, out var channel)
            ? channel
            : throw new ArgumentException($"No channel named '${channelName}' registered with this broker.");
    }
}
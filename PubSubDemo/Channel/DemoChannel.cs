using PubSubDemo.Message;
using PubSubDemo.Subscriber;

namespace PubSubDemo.Channel
{
    public class DemoChannel : IChannel
    {
        private readonly Dictionary<Type, HashSet<IMessageSubscriber>> _subscribers;

        private readonly Dictionary<(Type, Type), IChannelTransform> _channelTransforms;

        public string Name { get; private init; }

        public DemoChannel(string channelName)
        {
            Name = channelName;

            _channelTransforms = new Dictionary<(Type, Type), IChannelTransform>();
            _subscribers = new Dictionary<Type, HashSet<IMessageSubscriber>>();
        }

        public void Publish<T>(T message)
            where T : IMessage
        {
            var fromType = typeof(T);

            foreach (var ((from, toType), transform) in _channelTransforms)
            {
                if (from != fromType)
                    continue;

                // we always call the transform, even if there might be no subscribers, in case the transform keeps state
                if (!transform.Transform(message, out var result))
                    continue;

                if (_subscribers.TryGetValue(toType, out var subscribers))
                    foreach (var subscriber in subscribers)
                        subscriber.Receive(Name, result);
            }

            // if there is no endotransform defined we simply send the message unedited to subscribers of that type
            if (!_channelTransforms.ContainsKey((fromType, fromType)))
                if (_subscribers.TryGetValue(fromType, out var subscribers))
                    foreach (var subscriber in subscribers)
                        subscriber.Receive(Name, message);
        }

        public void AddOrReplaceChannelTransform<T1, T2>(AbstractChannelTransform<T1, T2> transform)
            where T1 : IMessage
            where T2 : IMessage =>
            _channelTransforms[(typeof(T1), typeof(T2))] = transform;

        public void RemoveChannelTransform<T1, T2>()
            where T1 : IMessage
            where T2 : IMessage =>
            _channelTransforms.Remove((typeof(T1), typeof(T2)));

        public void Subscribe<T>(AbstractSubscriber<T> subscriber) where T : IMessage
        {
            var messageType = typeof(T);

            if (_subscribers.TryGetValue(messageType, out var subscribers))
                subscribers.Add(subscriber);
            else
                _subscribers[messageType] = new HashSet<IMessageSubscriber> { subscriber };
        }

        public void Unsubscribe<T>(AbstractSubscriber<T> subscriber) where T : IMessage
        {
            var messageType = typeof(T);

            if (_subscribers.TryGetValue(messageType, out var subscribers))
                subscribers.Remove(subscriber);
        }
    }
}
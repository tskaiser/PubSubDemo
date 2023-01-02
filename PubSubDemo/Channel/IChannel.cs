using PubSubDemo.Message;
using PubSubDemo.Subscriber;

namespace PubSubDemo.Channel
{
    public interface IChannel
    {
        public string Name { get; }

        public void Publish<T>(T message)
            where T : IMessage;

        public void Subscribe<T>(AbstractSubscriber<T> subscriber)
            where T : IMessage;

        public void Unsubscribe<T>(AbstractSubscriber<T> subscriber)
            where T : IMessage;

        public void AddOrReplaceChannelTransform<T1, T2>(AbstractChannelTransform<T1, T2> transform)
            where T1 : IMessage
            where T2 : IMessage;

        public void RemoveChannelTransform<T1, T2>()
            where T1 : IMessage
            where T2 : IMessage;
    }
}

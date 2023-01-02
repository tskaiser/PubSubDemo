using PubSubDemo.Message;

namespace PubSubDemo.Subscriber
{
    public class LambdaMessageSubscriber<T> : AbstractSubscriber<T>
        where T : IMessage
    {
        private readonly Action<string, T> _receive;

        public LambdaMessageSubscriber(Action<string, T> receive) =>
            _receive = receive;

        public override void Receive(string channelName, T message) =>
            _receive(channelName, message);
    }
}
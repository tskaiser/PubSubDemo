using PubSubDemo.Message;
using System.Diagnostics.CodeAnalysis;

namespace PubSubDemo.Channel
{
    public class LambdaChannelTransform<T1, T2> : AbstractChannelTransform<T1, T2>
        where T1 : IMessage
        where T2 : IMessage
    {
        private readonly Func<T1, T2?> _transform;

        public LambdaChannelTransform(Func<T1, T2?> transform) => _transform = transform;

        public override bool Transform(T1 message, [MaybeNullWhen(false)] out T2 result)
        {
            if (_transform(message) is T2 msg)
            {
                result = msg;
                return true;
            }

            result = default;
            return false;
        }
    }
}
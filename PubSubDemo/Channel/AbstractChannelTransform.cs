using PubSubDemo.Message;
using System.Diagnostics.CodeAnalysis;

namespace PubSubDemo.Channel
{
    public abstract class AbstractChannelTransform<T1, T2> : IChannelTransform
        where T1 : IMessage
        where T2 : IMessage
    {
        public abstract bool Transform(T1 message, [MaybeNullWhen(false)] out T2 result);

        public bool Transform(IMessage message, [MaybeNullWhen(false)] out IMessage result)
        {
            if (message is T1 castedMessage && Transform(castedMessage, out var transformedResult))
            {
                result = transformedResult;
                return true;
            }

            result = default;
            return false;
        }
    }
}
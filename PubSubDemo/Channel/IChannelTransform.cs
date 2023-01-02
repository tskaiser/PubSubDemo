using PubSubDemo.Message;
using System.Diagnostics.CodeAnalysis;

namespace PubSubDemo.Channel
{
    public interface IChannelTransform
    {
        public bool Transform(IMessage message, [MaybeNullWhen(false)] out IMessage result);
    }
}
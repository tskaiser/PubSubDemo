using PubSubDemo.Broker;
using PubSubDemo.Channel;
using PubSubDemo.Message;
using PubSubDemo.Subscriber;

namespace PubSubDemoTest
{
    [TestClass]
    public class DemoTests
    {
        [TestMethod]
        public void CreateChannel()
        {
            const string channelName = "test";
            DemoBroker broker = new();

            var channel = broker.CreateChannel(channelName);

            Assert.IsNotNull(channel);
            Assert.AreEqual(channelName, channel.Name);
        }

        [TestMethod]
        public void CreateChannelThrowIfDuplicate()
        {
            const string channelName = "test channel";
            DemoBroker broker = new();

            broker.CreateChannel(channelName);
            Assert.ThrowsException<ArgumentException>(() => broker.CreateChannel(channelName));
        }

        [TestMethod]
        public void Publish()
        {
            const string channelName = "test channel";
            const string testMessage = "test message";
            DemoBroker broker = new();

            broker.CreateChannel(channelName);

            GenericMessage<string>? closure = null;
            LambdaMessageSubscriber<GenericMessage<string>> subscriber = new((_, msg) => closure = msg);
            broker.Subscribe(channelName, subscriber);

            Assert.IsNull(closure);

            broker.Publish<GenericMessage<string>>(channelName, new(testMessage));

            Assert.IsNotNull(closure);
            Assert.AreEqual(closure.Value, testMessage);
        }

        [TestMethod]
        public void PublishThrowIfNonexisting()
        {
            const string channelName = "test channel";
            DemoBroker broker = new();
            GenericMessage<int> dummyMsg = new(0);

            Assert.ThrowsException<ArgumentException>(() => broker.Publish(channelName, dummyMsg));
        }

        [TestMethod]
        public void SubscribeThrowIfNonexisting()
        {
            const string channelName = "test channel";
            DemoBroker broker = new();
            LambdaMessageSubscriber<IMessage> dummySubscriber = new((_, _) => { });

            Assert.ThrowsException<ArgumentException>(() =>
                broker.Subscribe(channelName, dummySubscriber));
        }

        [TestMethod]
        public void TransformAtoA()
        {
            const string channelName = "test channel";
            DemoBroker broker = new();
            var channel = broker.CreateChannel(channelName);

            static GenericMessage<int> MultTwo(GenericMessage<int> msg) =>
                new(msg.Value * 2);

            LambdaChannelTransform<GenericMessage<int>, GenericMessage<int>> transform =
                new(MultTwo);

            channel.AddOrReplaceChannelTransform(transform);

            List<int> closure = new();
            LambdaMessageSubscriber<GenericMessage<int>> subscriber =
                new((_, msg) => closure.Add(msg.Value));
            broker.Subscribe(channelName, subscriber);

            void Publish(int value) => broker.Publish(channelName, new GenericMessage<int>(value));

            Assert.AreEqual(0, closure.Count);

            Publish(1);
            Publish(2);
            Publish(3);

            Assert.AreEqual(3, closure.Count);
            Assert.AreEqual(2, closure[0]);
            Assert.AreEqual(4, closure[1]);
            Assert.AreEqual(6, closure[2]);
        }

        [TestMethod]
        public void TransformAtoB()
        {
            const string channelName = "test channel";
            DemoBroker broker = new();
            var channel = broker.CreateChannel(channelName);

            static GenericMessage<string> ToString(GenericMessage<int> msg) =>
                new(msg.Value.ToString());

            LambdaChannelTransform<GenericMessage<int>, GenericMessage<string>> transform = new(ToString);
            channel.AddOrReplaceChannelTransform(transform);

            List<int> original = new();
            LambdaMessageSubscriber<GenericMessage<int>> originalSubscriber =
                new((_, msg) => original.Add(msg.Value));

            broker.Subscribe(channelName, originalSubscriber);

            List<string> transformed = new();
            LambdaMessageSubscriber<GenericMessage<string>> transformedSubscriber =
                new((_, msg) => transformed.Add(msg.Value));
            
            broker.Subscribe(channelName, transformedSubscriber);

            void Publish(int value) => broker.Publish(channelName, new GenericMessage<int>(value));

            Assert.AreEqual(0, original.Count);
            Assert.AreEqual(0, transformed.Count);

            Publish(1);
            Publish(2);
            Publish(3);

            Assert.AreEqual(3, original.Count);
            Assert.AreEqual(3, transformed.Count);

            Assert.AreEqual(1, original[0]);
            Assert.AreEqual(2, original[1]);
            Assert.AreEqual(3, original[2]);

            Assert.AreEqual("1", transformed[0]);
            Assert.AreEqual("2", transformed[1]);
            Assert.AreEqual("3", transformed[2]);
        }

        [TestMethod]
        public void TransformAsFilter()
        {
            const string channelName = "test channel";
            DemoBroker broker = new();
            var channel = broker.CreateChannel(channelName);

            static GenericMessage<int>? RemoveOdds(GenericMessage<int> msg) =>
                msg.Value % 2 == 0 ? msg : null;

            LambdaChannelTransform<GenericMessage<int>, GenericMessage<int>> transform = new(RemoveOdds);
            channel.AddOrReplaceChannelTransform(transform);

            List<int> closure = new();
            LambdaMessageSubscriber<GenericMessage<int>> subscriber =
                new((_, msg) => closure.Add(msg.Value));
            broker.Subscribe(channelName, subscriber);

            int[] numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int[] expected = new[] { 2, 4, 6, 8, 10 };

            Assert.AreEqual(0, closure.Count);
            foreach (var number in numbers)
            {
                broker.Publish(channelName, new GenericMessage<int>(number));
            }

            Assert.AreEqual(closure.Count, expected.Length);
            for (var i = 0; i < closure.Count; i++)
            {
                Assert.AreEqual(closure[i], expected[i]);
            }
        }
    }
}
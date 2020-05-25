using NUnit.Framework;

namespace IsUakr.MessageBroker.Tests
{
    public class MqManagerTests
    {
        private IMqManager mqManager;

        [SetUp]
        public void Setup()
        {
            var mqService = new MqService(@"amqp://ilaoklcx:OfEMmmsEpt6hYDhL_jj3F19cv5H4idWL@squid.rmq.cloudamqp.com/ilaoklcx");
            mqManager = new MqManager(mqService);
        }

        [Test]
        public void MqManagerTest_Publish()
        {
            try
            {
                Assert.DoesNotThrow(() =>
                {
                    mqManager.PublishMessage("helloRabbit");
                });
            }
            finally
            {
                mqManager.DeleteQueues();
            }
        }

        [Test]
        public void MqManagerTest_Refresh()
        {
            try
            {
                Assert.DoesNotThrow(() =>
                {
                    mqManager.PublishMessage("helloRabbit");
                    mqManager.RefreshQueuesInfo();
                });
            }
            finally
            {
                mqManager.DeleteQueues();
            }
        }

        [Test]
        public void MqManagerTest_HighloadQueues()
        {
            uint messageCount = 102;
            uint messagesFromQueues = 0;
            try
            {
                Assert.DoesNotThrow(() =>
                {
                    for(var i = 0; i < messageCount; i++)
                        mqManager.PublishMessage("helloRabbit" + (i+1));
                    messagesFromQueues = mqManager.DeleteQueues();
                });
                
                Assert.That(messagesFromQueues, Is.EqualTo(messageCount));
            }
            finally
            {
                mqManager.DeleteQueues();
            }
        }
    }
}

using NUnit.Framework;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace IsUakr.MessageBroker.Tests
{
    public class MqManagerTests
    {
        private IMqManager mqManager;
        private string exchangeName;
        [SetUp]
        public void Setup()
        {
            var ass = Assembly.GetExecutingAssembly();
            var stream = ass.GetManifestResourceStream(ass.GetManifestResourceNames()[0]);
            var config = XDocument.Load(stream);

            var rabbitConn = config.Root.Element("connectionStrings").Element("add").Attribute("connectionString").Value;
            exchangeName = config.Root.Element("rabbitConfiguration").Element("exchangeName").Attribute("name").Value;
            var queues = config.Root.Element("rabbitConfiguration").Element("queues").Elements().Select(p => p.Attribute("value").Value);
            mqManager = new MqManager(new MqService(rabbitConn), new Helpers.QueueInfo(exchangeName, queues.ToList()));
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
                mqManager.DeleteExchange(exchangeName);
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
                mqManager.DeleteExchange(exchangeName);
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
                mqManager.DeleteExchange(exchangeName);
            }
        }
    }
}

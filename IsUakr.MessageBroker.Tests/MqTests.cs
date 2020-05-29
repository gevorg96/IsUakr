using NUnit.Framework;
using System;
using System.Reflection;
using System.Xml.Linq;

namespace IsUakr.MessageBroker.Tests
{
    public class MqTests
    {
        private IMqService mqService;

        [SetUp]
        public void Setup()
        {
            var ass = Assembly.GetExecutingAssembly();
            var stream = ass.GetManifestResourceStream(ass.GetManifestResourceNames()[0]);
            var config = XDocument.Load(stream);

            var rabbitConn = config.Root.Element("connectionStrings").Element("add").Attribute("connectionString").Value;
            mqService = new MqService(rabbitConn);
        }

        private MqQueueInfo GenerateQueueInfo() => new MqQueueInfo { ExchangeName = Guid.NewGuid().ToString(), RoutingKey = Guid.NewGuid().ToString(), QueueName = Guid.NewGuid().ToString() };

        [Test]
        public void MqTest_CreateQueue()
        {
            var mqInfo = GenerateQueueInfo();

            try
            {
                Assert.DoesNotThrow(() => 
                {
                    mqService.CreateRabbitQueue(mqInfo, false);
                });
            }
            finally
            {
                mqService.DeleteQueue(mqInfo.QueueName);
            }
        }

        [Test]
        public void MqTest_SendMessage()
        {
            var mqInfo = GenerateQueueInfo();

            try
            {
                Assert.DoesNotThrow(() =>
                {
                    mqService.CreateRabbitQueue(mqInfo, false);
                    mqService.Send("helloRabbit", mqInfo);
                });
            }
            finally
            {
                mqService.DeleteQueue(mqInfo.QueueName);
            }
        }

        [Test]
        public void MqTest_ReceiveMessage()
        {
            var mqInfo = GenerateQueueInfo();
            string message = "helloRabbit";

            try
            {
                string receivedMessage = null;
                Assert.DoesNotThrow(() =>
                {
                    mqService.CreateRabbitQueue(mqInfo, false);
                    mqService.Send(message, mqInfo);
                    receivedMessage = mqService.Receive(mqInfo.QueueName);
                });
                
                Assert.IsNotNull(receivedMessage);
                Assert.That(receivedMessage, Is.EqualTo(message));
            }
            finally
            {
                mqService.DeleteQueue(mqInfo.QueueName);
            }
        }

        [Test]
        public void MqTest_ReceiveMessages()
        {
            var mqInfo = GenerateQueueInfo();
            string firstMessage = "helloRabbit1";
            string secondMessage = "helloRabbit2";

            try
            {
                string firstReceivedMessage = null;
                string secondReceivedMessage = null;
                Assert.DoesNotThrow(() =>
                {
                    mqService.CreateRabbitQueue(mqInfo, false);
                    mqService.Send(firstMessage, mqInfo);
                    mqService.Send(secondMessage, mqInfo);

                    firstReceivedMessage = mqService.Receive(mqInfo.QueueName);
                    secondReceivedMessage = mqService.Receive(mqInfo.QueueName);
                });

                Assert.IsNotNull(firstReceivedMessage);
                Assert.IsNotNull(secondReceivedMessage);
                Assert.That(firstReceivedMessage, Is.EqualTo(firstMessage));
                Assert.That(secondReceivedMessage, Is.EqualTo(secondMessage));
            }
            finally
            {
                mqService.DeleteQueue(mqInfo.QueueName);
            }
        }

        [Test]
        public void MqTest_DeleteQueue()
        {
            var mqInfo = GenerateQueueInfo();
            string firstMessage = "helloRabbit1";
            string secondMessage = "helloRabbit2";

            try
            {
                uint messageCount = 0;
                Assert.DoesNotThrow(() =>
                {
                    mqService.CreateRabbitQueue(mqInfo, false);
                    mqService.Send(firstMessage, mqInfo);
                    mqService.Send(secondMessage, mqInfo);

                    messageCount = mqService.DeleteQueue(mqInfo.QueueName);
                });

                Assert.That(messageCount, Is.EqualTo(2));
            }
            finally
            {
                var isDelete = mqService.DeleteQueue(mqInfo.QueueName);
            }
        }
    }
}
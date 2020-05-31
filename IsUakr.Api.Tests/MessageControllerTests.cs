using IsUakr.Entities.Messages;
using IsUakr.MessageBroker;
using IsUakr.Mvc.Controllers;
using NUnit.Framework;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IsUakr.Api.Tests
{
    public class MessageControllerTests
    {
        private MessageController _messageController;
        private IMqManager _mqManager;
        private string _exchangeName;
        [SetUp]
        public void Setup()
        {
            var ass = Assembly.GetExecutingAssembly();
            var stream = ass.GetManifestResourceStream(ass.GetManifestResourceNames()[0]);
            var config = XDocument.Load(stream);

            var rabbitConn = config.Root.Element("connectionStrings").Element("add").Attribute("connectionString").Value;
            _exchangeName = config.Root.Element("rabbitConfiguration").Element("exchangeName").Attribute("name").Value;
            var queues = config.Root.Element("rabbitConfiguration").Element("queues").Elements().Select(p => p.Attribute("value").Value);
            _mqManager = new MqManager(new MqService(rabbitConn), new MessageBroker.Helpers.QueueInfo(_exchangeName, queues.ToList()));
            _messageController = new MessageController(_mqManager);
        }

        [Test]
        public void MessageController_Load()
        {
            try
            {
                var state = new StateMessage { MeasureUnit = 1, Message = "OK", MeterDt = DateTime.Now, State = 1, Volume = 200 };
                var meterMess = new MeterMessage { Id = 1, Address = 234, Env = 0, Maker = "lora", SerialNumber = 1, SetupIdentity = 243, Body = state };
                var hubMess = new HubMessage { HubId = 1, Messages = new System.Collections.Generic.List<MeterMessage> { meterMess } };

                Assert.DoesNotThrowAsync(async () =>
                {
                    await _messageController.Load(hubMess).ConfigureAwait(false);
                });
            }
            finally
            {
                _mqManager.DeleteQueues();
                _mqManager.DeleteExchange(_exchangeName);
            }
        }
    }
}
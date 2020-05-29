using IsUakr.MessageHandler.DAL;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IsUakr.MessageHandler.Tests
{
    public class WorkerTests
    {
        private string _dbConnStr;
        [SetUp]
        public void Setup()
        {
            var ass = Assembly.GetExecutingAssembly();
            var stream = ass.GetManifestResourceStream(ass.GetManifestResourceNames()[0]);
            var config = XDocument.Load(stream);

            _dbConnStr = config.Root.Element("connectionStrings").Elements("add").First(x => x.Attribute("name").Value == "isuakr_db").Attribute("connectionString").Value;
        }

        [Test]
        public async Task MakeDecision_Test()
        {
            var flatFinder = new FlatDecisionMaker(_dbConnStr);
            var tableName = "flat_26829";
            try
            {
                var newTableName = string.Empty;
                Assert.DoesNotThrowAsync(async () =>
                {
                    await flatFinder.UsingConnectionAsync(async (conn) =>
                    {
                        newTableName = await flatFinder.MakeDecision(conn, 80055);
                    });
                });

                Assert.That(newTableName, Is.EqualTo(tableName));
            }
            finally
            {
                await flatFinder.TableDelete("flat_26829");
            }
        }

        [Test]
        public async Task Processor_Test()
        {
            var mess = "{\"HubId\":1,\"Messages\":[" +
                "{\"Id\":341,\"Address\":234,\"Maker\":\"lora\",\"SetupIdentity\":243,\"Env\":0,\"SerialNumber\":1,\"Body\":{\"State\":1,\"Message\":\"OK\",\"MeterDt\":\"2020-05-27T19:57:19.1639415+03:00\",\"Volume\":200.0,\"MeasureUnit\":1}}," +
                "{\"Id\":340,\"Address\":234,\"Maker\":\"lora\",\"SetupIdentity\":243,\"Env\":0,\"SerialNumber\":1,\"Body\":{\"State\":1,\"Message\":\"OK\",\"MeterDt\":\"2020-05-27T19:57:20.1639415+03:00\",\"Volume\":200.0,\"MeasureUnit\":1}}," +
                "{\"Id\":339,\"Address\":234,\"Maker\":\"lora\",\"SetupIdentity\":243,\"Env\":0,\"SerialNumber\":1,\"Body\":{\"State\":1,\"Message\":\"OK\",\"MeterDt\":\"2020-05-27T19:57:21.1639415+03:00\",\"Volume\":200.0,\"MeasureUnit\":1}}," +
                "{\"Id\":338,\"Address\":234,\"Maker\":\"lora\",\"SetupIdentity\":243,\"Env\":0,\"SerialNumber\":1,\"Body\":{\"State\":1,\"Message\":\"OK\",\"MeterDt\":\"2020-05-27T19:57:22.1639415+03:00\",\"Volume\":200.0,\"MeasureUnit\":1}}" +
                "]}";

            var flatFinder = new FlatDecisionMaker(_dbConnStr);
            var processor = new MessageProcessor(flatFinder);
            var output = string.Empty;
            try
            {
                Assert.DoesNotThrowAsync(async () =>
                {
                    output = await processor.Process(mess);
                });

                var table1Count = 0;
                var table2Count = 1;

                await flatFinder.UsingConnectionAsync(async (conn) =>
                {
                    table1Count = await flatFinder.GetRowsCount(conn, "flat_2");
                    table2Count = await flatFinder.GetRowsCount(conn, "flat_3");
                });

                Assert.That(output, Is.EqualTo(" оличество обработанных сообщений: 4"));
                Assert.That(table1Count, Is.EqualTo(3));
                Assert.That(table2Count, Is.EqualTo(1));
            }
            finally
            {
                await flatFinder.TableDelete("flat_2");
                await flatFinder.TableDelete("flat_3");
            }
        }
    }
}
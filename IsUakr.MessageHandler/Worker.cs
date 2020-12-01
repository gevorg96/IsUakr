using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IsUakr.MessageHandler.DAL;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace IsUakr.MessageHandler
{
    public class Worker : BackgroundService
    {
        private readonly string _queueName;
        private readonly string _mqConnStr;
        private readonly IProcessor _processor;
        private static ConnectionFactory _factory;
        private bool _stop = false;
        public Worker(ConnStrProvider queue, IProcessor processor)
        {
            _queueName = queue.QueueName;
            _mqConnStr = queue.MqConnStr;
            _processor = processor;
        }


        public IModel GetChannel()
        {
            return CreateFactory().CreateConnection().CreateModel();
        }

        private ConnectionFactory CreateFactory()
        {
            if (_factory == null)
            {
                _factory = new ConnectionFactory
                {
                    Uri = new Uri(_mqConnStr)
                };
            }

            return _factory;
        }

        public void Stop() => _stop = true;

        public async Task Execute()
        {
            await ExecuteAsync(new CancellationToken(false));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var conn = CreateFactory().CreateConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += async (model, ea) =>
                    {
                        var body = ea.Body;
                        
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine(" [x] {0}", message);
                        Console.WriteLine(await _processor.Process(message));
                    };
                    channel.BasicConsume(queue: _queueName,
                                         autoAck: false,
                                         consumer: consumer);

                    while (_stop)
                    {
                    
                    }
                    channel.QueueDelete(_queueName, false, true);
                }
            }
        }
    }
}

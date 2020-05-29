using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IsUakr.MessageBroker;
using IsUakr.MessageHandler.DAL;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace IsUakr.MessageHandler
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMqService _mqService;
        private readonly string _queueName;
        private readonly IDecisionMaker _decisionMaker;
        private readonly IProcessor _processor;

        public Worker(ILogger<Worker> logger, IMqService mqService, Queue queue, IDecisionMaker decisionMaker, IProcessor processor)
        {
            _logger = logger;
            _mqService = mqService;
            _queueName = queue.QueueName;
            _decisionMaker = decisionMaker;
            _processor = processor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _mqService.UsingConnection(channel =>
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
                                     autoAck: true,
                                     consumer: consumer);

                while (!stoppingToken.IsCancellationRequested)
                { }
            });
        }
    }
}

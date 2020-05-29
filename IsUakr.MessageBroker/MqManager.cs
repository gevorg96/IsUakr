using IsUakr.MessageBroker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsUakr.MessageBroker
{
    public class MqManager: IMqManager
    {
        private readonly IMqService mqService;
        private List<MqQueueInfo> exchangesQueues;
        private readonly QueueInfo _queueInfo;
        private static object _lock = new object();

        public MqManager(IMqService service, QueueInfo queueInfo)
        {
            mqService = service;
            _queueInfo = queueInfo;
            exchangesQueues = new List<MqQueueInfo>();
        }

        private MqQueueInfo CreateNewQueue()
        {
            var q = _queueInfo.Queues.ToList();
            q.RemoveAll(p => exchangesQueues.Select(x => x.QueueName).Contains(p));
            var queueName = q.FirstOrDefault();

            var mqInfo = new MqQueueInfo 
            { 
                ExchangeName = _queueInfo.ExchangeName,
                RoutingKey = queueName, 
                QueueName = queueName 
            };

            mqService.CreateRabbitQueue(mqInfo, exchangesQueues.Count() != 0);
            exchangesQueues.Add(mqInfo);
            return mqInfo;
        }

        public void PublishMessage(string message)
        {
            try
            {
                MqQueueInfo queue = null;
                lock (_lock)
                {
                    queue = exchangesQueues.FirstOrDefault(x => x.MessageCount < 100);

                    if (queue == null)
                    {
                        if (exchangesQueues.Count < 4)
                        {
                            queue = CreateNewQueue();
                        }
                        else
                        {
                            throw new Exception("Ошибка при создании дополнительной очереди. Необходимо расширить кластер воркеров по обработке данных.");
                        }
                    }
                }

                mqService.Send(message, queue);
                queue.MessageCount++;
            }
            catch (Exception ex)
            {
                throw new Exception("При отправке сообщения произошла ошибка.\n Message: " + ex.Message);
            }
        }

        public void RefreshQueuesInfo()
        {
            mqService.UsingConnection(channel =>
            {
                foreach (var queue in exchangesQueues)
                {
                    var result = mqService.DeclareQueue(channel, queue.QueueName);
                    queue.MessageCount = result.MessageCount;
                }
            });
        }

        public void ClearQueues()
        {
            foreach (var queue in exchangesQueues)
            {
                mqService.DeleteQueue(queue.QueueName);
                queue.MessageCount = 0;
            }
        }

        public uint DeleteQueues()
        {
            uint messageCount = 0;
            foreach (var queue in exchangesQueues)
            {
                messageCount += mqService.DeleteQueue(queue.QueueName);
            }

            if (exchangesQueues.FirstOrDefault() != null)
                DeleteExchange(exchangesQueues.FirstOrDefault().ExchangeName);

            exchangesQueues = new List<MqQueueInfo>();
            return messageCount;
        }

        public void DeleteExchange(string exchangeName)
        {
            mqService.UsingConnection(channel =>
            {
                channel.ExchangeDelete(exchangeName, false);
            });
        }
    }
}

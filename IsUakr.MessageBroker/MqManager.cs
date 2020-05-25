using System;
using System.Collections.Generic;
using System.Linq;

namespace IsUakr.MessageBroker
{
    public class MqManager: IMqManager
    {
        private IMqService mqService;
        private List<MqQueueInfo> exchangesQueues;

        public MqManager(IMqService service)
        {
            mqService = service;
            exchangesQueues = new List<MqQueueInfo>();
        }

        private MqQueueInfo CreateNewQueue()
        {
            var queue = exchangesQueues.FirstOrDefault();
            var mqInfo = new MqQueueInfo 
            { 
                ExchangeName = Guid.NewGuid().ToString(), 
                RoutingKey = Guid.NewGuid().ToString(), 
                QueueName = Guid.NewGuid().ToString() 
            };

            if (queue != null)
                mqInfo.ExchangeName = queue.ExchangeName;

            mqService.CreateRabbitQueue(mqInfo, exchangesQueues.Count() != 0);
            exchangesQueues.Add(mqInfo);
            return mqInfo;
        }

        public void PublishMessage(string message)
        {
            try
            {
                var queue = exchangesQueues.FirstOrDefault(x => x.MessageCount < 100);

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

            exchangesQueues = new List<MqQueueInfo>();
            return messageCount;
        }
    }
}

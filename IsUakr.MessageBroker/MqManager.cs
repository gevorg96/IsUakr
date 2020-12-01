using IsUakr.MessageBroker.Commands;
using IsUakr.MessageBroker.Helpers;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsUakr.MessageBroker
{
    public class MqManager: IMqManager
    {
        private readonly IMqService mqService;
        private static List<MqQueueInfo> exchangesQueues;
        private static IModel model;
        private static object _lock = new object();
        
        public MqManager(IMqService service, QueueInfo queueInfo)
        {
            mqService = service;
            //_queueInfo = queueInfo;
            exchangesQueues = new List<MqQueueInfo>();
            foreach (var queue in queueInfo.Queues)
            {
                exchangesQueues.Add(new MqQueueInfo { ExchangeName = queueInfo.ExchangeName, RoutingKey = queue, QueueName = queue });
            }
            RefreshQueuesInfo();
        }

        public string PublishMessage(string message)
        {
            try
            {
                List<QueueDeclareOk> results = new List<QueueDeclareOk>();
                var queueName = "queue0";
                bool res = false;
                var command = new Command();

                lock (_lock)
                {
                    if (model == null)
                        model = mqService.GetChannel();

                    var result = model.QueueDeclare(queueName, true, false, false, null);
                    if (result.MessageCount >= 30)
                    {
                        for (int i = 1; i < 13; i++)
                        {
                            result = model.QueueDeclare("queue" + i);

                            if (result.MessageCount < 30)
                            {
                                if (result.MessageCount == 0)
                                    command.Add(WhatToDo.Add, result.QueueName);
                                else
                                    command.Add(WhatToDo.Nothing, result.QueueName);
                                queueName = result.QueueName;
                            }
                        }
                    }

                    model.QueueBind(result.QueueName, "exchange", result.QueueName, null);

                    mqService.Send(model, message, result.QueueName, "exchange");
                    return res ? result.QueueName : string.Empty;
                }
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

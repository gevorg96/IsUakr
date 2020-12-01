using RabbitMQ.Client;
using System;

namespace IsUakr.MessageBroker
{
    public interface IMqService
    {
        IModel GetChannel();
        void Send(IModel model, string data, string queueName, string exchageName);
        void CreateRabbitQueue(MqQueueInfo mqInfo, bool exchangeExist);
        void Send(string data, MqQueueInfo mqInfo);
        string Receive(string queueName);
        uint DeleteQueue(string queueName);
        void DeleteExchange(string exchangeName);
        void UsingConnection(Action<IModel> action);
        QueueDeclareOk DeclareQueue(IModel channel, string queueName, 
            bool durable = true, bool exclusive = false, bool autoDelete = false);
    }
}

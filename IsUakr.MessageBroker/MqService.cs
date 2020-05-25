using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace IsUakr.MessageBroker
{
    public class MqService: IMqService
    {
        private static ConnectionFactory _factory;
        private static string _uri;
        public MqService(string uri)
        {
            _uri = uri;
        }

        private ConnectionFactory CreateFactory()
        {
            if (_factory == null)
            {
                _factory = new ConnectionFactory();
                _factory.Uri = new Uri(_uri);
            }

            return _factory;
        }

        public void CreateRabbitQueue(MqQueueInfo mqInfo, bool exchangeExist)
        {
            try
            {
                (string exchangeName, string routingKey, string queueName, bool durable, bool exclusive, bool autoDelete) = mqInfo;

                using (var conn = CreateFactory().CreateConnection())
                {
                    using (var channel = conn.CreateModel())
                    {
                        if(!exchangeExist)
                            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
                        DeclareQueue(channel, queueName, durable, exclusive, autoDelete);
                        channel.QueueBind(queueName, exchangeName, routingKey, null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка во время создания очереди.\n Message: " + ex.Message);
            }
        }

        public void Send(string data, MqQueueInfo mqInfo)
        {
            (string exchangeName, string routingKey, string queueName, bool durable, bool exclusive, bool autoDelete) = mqInfo;
            try
            {
                using (IConnection connection = CreateFactory().CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        //if(queueName != null)
                        //    DeclareQueue(channel, queueName, durable, exclusive, autoDelete);
                        
                        channel.BasicPublish(exchangeName, routingKey, null, Encoding.UTF8.GetBytes(data));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка во время отправки сообщения в очередь.\n Message: " + ex.Message);
            }

        }

        public string Receive(string queueName)
        {
            try
            {
                using (IConnection connection = CreateFactory().CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var consumer = new EventingBasicConsumer(channel);
                        var result = channel.BasicGet(queueName, true);

                        if (result != null)
                            return Encoding.UTF8.GetString(result.Body.ToArray());

                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка во время получения сообщения в очередь.\n Message: " + ex.Message);
            }
        }

        public uint DeleteQueue(string queueName)
        {
            try
            {
                using (IConnection connection = CreateFactory().CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        return channel.QueueDelete(queueName, false, false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка во время удаления очереди.\n Message: " + ex.Message);
            }
        }

        public void DeleteExchange(string exchangeName)
        {
            try
            {
                using (IConnection connection = CreateFactory().CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDelete(exchangeName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка во время удаления обменника.\n Message: " + ex.Message);
            }
        }

        public void UsingConnection(Action<IModel> action)
        {
            try
            {
                using (IConnection connection = CreateFactory().CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        action(channel);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Возникла ошибка во время выполнения действия в MqService.\n Message: " + ex.Message);
            }
        }

        public QueueDeclareOk DeclareQueue(IModel channel, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false) => 
            channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);
    }
}

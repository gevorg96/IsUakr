namespace IsUakr.MessageBroker
{
    public class MqQueueInfo
    {
        public string ExchangeName { get; set; }

        public string RoutingKey { get; set; }

        public string QueueName { get; set; }

        public uint MessageCount { get; set; } = 0;

        public bool Durable { get; set; } = true;

        public bool Exclusive { get; set; } = false;

        public bool AutoDelete { get; set; } = false;

        public void Deconstruct(out string exchangeName, out string routingKey, out string queueName, out bool durable, out bool exclusive, out bool autoDelete)
        {
            exchangeName = ExchangeName;
            routingKey = RoutingKey;
            queueName = QueueName;
            durable = Durable;
            exclusive = Exclusive;
            autoDelete = AutoDelete;
        }

        public void Deconstruct(out string exchangeName, out string routingKey, string queueName, uint messageCount)
        {
            exchangeName = ExchangeName;
            routingKey = RoutingKey;
            queueName = QueueName;
            messageCount = MessageCount;
        }
    }
}

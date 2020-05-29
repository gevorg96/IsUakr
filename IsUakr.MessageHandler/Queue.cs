namespace IsUakr.MessageHandler
{
    public class Queue
    {
        public string QueueName { get; }

        public string DbConnStr { get; }

        public Queue(string queueName, string dbConnStr)
        {
            QueueName = queueName;
            DbConnStr = dbConnStr;
        }
    }
}

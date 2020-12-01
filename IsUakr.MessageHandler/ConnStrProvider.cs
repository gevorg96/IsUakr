namespace IsUakr.MessageHandler
{
    public class ConnStrProvider
    {
        public string QueueName { get; }

        public string DbConnStr { get; }

        public string MqConnStr { get; }

        public ConnStrProvider(string queueName, string dbConnStr, string mqConnStr)
        {
            QueueName = queueName;
            DbConnStr = dbConnStr;
            MqConnStr = mqConnStr;
        }
    }
}

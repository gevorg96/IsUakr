﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IsUakr.MessageBroker.Helpers
{
    public class QueueInfo 
    {
        public readonly string ExchangeName;
        public readonly IReadOnlyCollection<string> Queues;

        public QueueInfo(string exchangeName, IList<string> queues)
        {
            ExchangeName = exchangeName;
            Queues = new ReadOnlyCollection<string>(queues);
        }
    }
}

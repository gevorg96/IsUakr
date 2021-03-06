﻿using System.Collections.Generic;

namespace IsUakr.MessageBroker
{
    public interface IMqManager
    {
        string PublishMessage(string message);
        
        void RefreshQueuesInfo();

        void ClearQueues();

        uint DeleteQueues();

        void DeleteExchange(string exchangeName);
    }
}

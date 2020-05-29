using System.Collections.Generic;

namespace IsUakr.MessageBroker
{
    public interface IMqManager
    {
        void PublishMessage(string message);
        
        void RefreshQueuesInfo();

        void ClearQueues();

        uint DeleteQueues();

        void DeleteExchange(string exchangeName);
    }
}

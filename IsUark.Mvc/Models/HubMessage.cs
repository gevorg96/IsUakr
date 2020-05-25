using System.Collections.Generic;

namespace IsUark.Mvc.Models
{
    public class HubMessage
    {
        public long HubId { get; set; }
        public List<MeterMessage> Messages { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsUark.Mvc.Models
{
    public class HubMessage
    {
        public long HubId { get; set; }
        public List<MeterMessage> Messages { get; set; }
    }
}

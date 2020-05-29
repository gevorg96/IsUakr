using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IsUakr.Entities.Messages
{
    public class HubMessage
    {
        [Required]
        public long HubId { get; set; }
        [Required]
        public List<MeterMessage> Messages { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

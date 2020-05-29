using System.ComponentModel.DataAnnotations;

namespace IsUakr.Entities.Messages
{
    public class StateMessage
    {
        [Required]
        public short State { get; set; }
        [Required]
        public string Message { get; set; }
        public System.DateTime? MeterDt { get; set; }
        public float? Volume { get; set; }
        public short? MeasureUnit { get; set; }
    }
}

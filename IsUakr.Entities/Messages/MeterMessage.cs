using System.ComponentModel.DataAnnotations;

namespace IsUakr.Entities.Messages
{
    public class MeterMessage
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Address { get; set; }
        [Required]
        public string Maker { get; set; }
        [Required]
        public int SetupIdentity { get; set; }
        [Required]
        public int Env { get; set; }
        [Required]
        public int SerialNumber { get; set; }
        [Required]
        public StateMessage Body { get; set; }
    }
}

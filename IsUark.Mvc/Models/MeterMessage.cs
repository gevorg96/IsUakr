namespace IsUark.Mvc.Models
{
    public class MeterMessage
    {
        public int Id { get; set; }
        public int Address { get; set; }
        public string Maker { get; set; }
        public int SetupIdentity { get; set; }
        public int Env { get; set; }
        public int SerialNumber { get; set; }
        public StateMessage Message { get; set; }
    }
}

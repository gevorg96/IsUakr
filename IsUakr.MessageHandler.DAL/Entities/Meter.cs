namespace IsUakr.MessageHandler.DAL.Entities
{
    public class Meter
    {
        public int Id { get; set; }
        public int FlatId { get; set; }
        public int HubId { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
    }
}

namespace IsUark.Mvc.Models
{
    public class StateMessage
    {
        public short State { get; set; }
        public string Message { get; set; }
        public System.DateTime? MeterDt { get; set; }
        public float? Volume { get; set; }
        public short? MeasureUnit { get; set; }
    }
}

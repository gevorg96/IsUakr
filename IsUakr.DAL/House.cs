namespace DAL
{
    public class House
    {
        public int id { get; set; }
        public string num { get; set; }
        public virtual Street Street{ get; set; }
        public int typeId { get; set; }
    }
}
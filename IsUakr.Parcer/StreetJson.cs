namespace IsUakr.Parcer
{
    public class StreetJson
    {
        public int id { get; set; }
        public string name { get; set; }
        public string socr { get; set; }

        public override string ToString()
        {
            return id + "\t" + name + "\t" + socr;
        }
    }
}
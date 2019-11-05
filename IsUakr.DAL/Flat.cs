using System.Collections.Generic;

namespace IsUakr.DAL
{
    public class Flat
    {
        public int id { get; set; }
        public House House { get; set; }
        public List<Meter> Meters { get; set; }
        public string num { get; set; }
    }
}
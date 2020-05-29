using IsUakr.DAL;
using System.Collections.Generic;
using System.Linq;

namespace IsUakr.HighLoad.Ui.Helpers
{
    public class Grouping
    {
        public Street Street { get; set; }
        public House House { get; set; }
        public bool IsFullStreet { get; set; }
        public bool IsFullHouse { get; set; }
        public List<Flat> Flats { get; set; }

        public Grouping()
        {
            Flats = new List<Flat>();
        }
    }
}

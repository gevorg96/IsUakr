using System.Collections.Generic;

namespace IsUakr.DAL
{
    public class Street
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public List<House> Houses { get; set; }

        public Street()
        {
            Houses = new List<House>();
        }
    }
}
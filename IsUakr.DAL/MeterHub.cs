using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsUakr.DAL
{
    public class MeterHub
    {
        public int id { get; set; }
        public House House { get; set; }
        public List<Meter> Meters { get; set; }
        public string code { get; set; }
       
    }
}
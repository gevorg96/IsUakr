using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsUakr.DAL
{
    public class Flat
    {
        [Column("id")]
        public int Id { get; set; }
        public House House { get; set; }
        public List<Meter> Meters { get; set; }
        [Column("num")]
        public string Num { get; set; }
    }
}
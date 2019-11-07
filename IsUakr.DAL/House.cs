using System;
using System.Collections.Generic;

namespace IsUakr.DAL
{
    public class House
    {
        public int id { get; set; }
        public string number { get; set; }
        public double? square{ get; set; }
        public int? year { get; set; }
        public int? floors { get; set; }
        public int code { get; set; }
        public int? flatsCount { get; set; }
        public DateTime? manageStartDate { get; set; }
        public List<Flat> Flats { get; set; }
        public Street Street { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", id, number, square, year, floors, code,
                flatsCount, manageStartDate);
        }
    }
}
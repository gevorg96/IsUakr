using System;
using System.Text;

namespace IsUakr.Parcer
{
    public class HouseJson
    {
        public int id { get; set; }
        public string num { get; set; }
        public int areaId { get; set; }
        public int cityId { get; set; }
        public int streetId { get; set; }
        public int type { get; set; }
        public string foto { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(id + "\t\t" + num + "\t\t" + areaId + "\t\t" + cityId + "\t\t" + streetId + "\t\t" + type);
            return sb.ToString();
        }
    }
}
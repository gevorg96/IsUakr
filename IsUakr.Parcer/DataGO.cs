using System.Collections.Generic;

namespace IsUakr.Parcer
{
    public class DataGO
    {
        public int current { get; set; }
        public int rowCount { get; set; }
        public RowGO[] rows { get; set; }
        public int total { get; set; }
    }
}
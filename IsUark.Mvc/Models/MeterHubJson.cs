using System.Collections.Generic;

namespace IsUark.Mvc.Models
{
    public class MeterHubJson
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public List<FlatJson> Flats { get; set; }
    }

    public class FlatJson
    {
        public int Id { get; set; }
        public string Num { get; set; }
        public List<MeterJson> Meters { get; set; }
    }

    public class MeterJson
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
    }
}
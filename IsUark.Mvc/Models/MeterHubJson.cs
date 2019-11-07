using System.Collections.Generic;

namespace IsUark.Mvc.Models
{
    public class MeterHubJson
    {
        public int id { get; set; }
        public string code { get; set; }
        public List<FlatJson> flats { get; set; }
    }

    public class FlatJson
    {
        public int id { get; set; }
        public string num { get; set; }
        public List<MeterJson> meters { get; set; }
    }

    public class MeterJson
    {
        public int id { get; set; }
        public string code { get; set; }
        public string type { get; set; }
    }
}
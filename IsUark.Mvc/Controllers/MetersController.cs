using System.Collections.Generic;
using System.Linq;
using IsUakr.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace IsUark.Mvc.Controllers
{
    [Route("api/meters")]
    public class MetersController
    {
        private readonly NpgDbContext _db;

        public MetersController(NpgDbContext db)
        {
            _db = db;
        }
        
        [HttpGet("{id}")]
        public ActionResult<MeterHub> GetMetersHubWithMeters(int? id)
        {
            var meterHub = _db.MeterHubs.Include(p => p.Meters).FirstOrDefault(p => p.House.id == id);
            var json = new JObject();
            json.Add(new JProperty("id", meterHub.id));
            json.Add(new JProperty("code", meterHub.code));
            json.Add("meters", GetArray(meterHub.Meters.ToList()));
            
            return json.ToObject<MeterHub>();
        }

        private JArray GetArray(List<Meter> Meters)
        {
            var jarray = new JArray();
            foreach (var meter in Meters)
            {
                var jobj = new JObject();
                jobj.Add(new JProperty("id", meter.id));
                jobj.Add(new JProperty("code", meter.code));
                jobj.Add(new JProperty("type", meter.type));
                
                jarray.Add(jobj);
            }

            return jarray;
        }
    }
}
using System.Linq;
using IsUakr.DAL;
using IsUark.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public ActionResult<MeterHubJson> GetMetersHubWithMeters(int? id)
        {
            var hub = _db.MeterHubs.Include(p => p.Meters).FirstOrDefault(p => p.House.id == id);
            var meters = _db.Meters.Include(p => p.Flat).Where(p => p.Hub.id == hub.id).ToList();
            var flats = meters.Select(p => p.Flat).Distinct().OrderBy(p => p.Num).ToList();

            var meterHubJson = new MeterHubJson
            {
                Id = hub.id,
                Code = hub.code,
                Flats = flats.Select(p =>
                        new FlatJson
                        {
                            Id = p.Id,
                            Num = p.Num,
                            Meters = p.Meters
                                .Select(q => new MeterJson {Id = q.id, Code = q.code, Type = q.type})
                                .ToList()
                        })
                    .ToList()
            };

            return meterHubJson;
        }
    }
}
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
            var flats = meters.Select(p => p.Flat).Distinct().OrderBy(p => p.num).ToList();

            var meterHubJson = new MeterHubJson
            {
                id = hub.id,
                code = hub.code,
                flats = flats.Select(p =>
                        new FlatJson
                        {
                            id = p.id,
                            num = p.num,
                            meters = p.Meters
                                .Select(q => new MeterJson {id = q.id, code = q.code, type = q.type})
                                .ToList()
                        })
                    .ToList()
            };

            return meterHubJson;
        }
    }
}
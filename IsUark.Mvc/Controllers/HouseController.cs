using System.Collections.Generic;
using System.Linq;
using IsUakr.DAL;
using Microsoft.AspNetCore.Mvc;

namespace IsUark.Mvc.Controllers
{
    [Route("api/houses")]
    public class HouseController : Controller
    {
        private readonly NpgDbContext _db;

        public HouseController(NpgDbContext db)
        {
            _db = db;
        }
        
        [HttpGet("{id}")]
        public ActionResult<List<House>> GetHousesByStreetId(int? id)
        {
            return _db.Houses.Where(p => p.Street.id == id).ToList();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IsUakr.DAL;
using IsUakr.Parcer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IsUark.Mvc.Controllers
{
    [Route("api/houses")]
    public class AjaxController : Controller
    {
        private readonly NpgDbContext _db;

        public AjaxController(NpgDbContext db)
        {
            _db = db;
            
        }
        
        [HttpGet("{id}")]
        public ActionResult<List<House>> GetHousesByStreetId(int? id)
        {
            return null; //_db.Houses.Where(p => p.Street.id == id).ToList();
        }
    }
}
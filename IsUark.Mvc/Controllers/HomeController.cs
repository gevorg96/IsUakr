using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IsUakr.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IsUark.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace IsUark.Mvc.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NpgDbContext _db;
        public HomeController(ILogger<HomeController> logger, NpgDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Streets.Include(p => p.Houses).ToList());
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
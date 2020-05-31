using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IsUakr.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IsUakr.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace IsUakr.Mvc.Controllers
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
            var streets = _db.Streets.Include(p => p.Houses).ToList();
            var hub = _db.MeterHubs.FirstOrDefault(p => p.House.id == streets.FirstOrDefault().Houses.FirstOrDefault().id);
            var meters = _db.Meters.Include(p => p.Flat).Where(p => p.Hub.id == hub.id).ToList();
            var flats = meters.Select(p => p.Flat).Distinct().OrderBy(p => p.Num).ToList();
            var vm = new AggregateViewModel
            {
                Streets = streets,
                Hub = hub,
                Flats = flats
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
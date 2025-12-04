using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models;
using MotoMarket.Web.Services;
using System.Diagnostics;

namespace MotoMarket.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVehicleService _vehicleService;

        public HomeController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public async Task<IActionResult> Index()
        {
            // Pobieramy auta z API
            var listings = await _vehicleService.GetAllListings();

            // Przekazujemy do widoku
            return View(listings);
        }

        // ... Error() itp.
    }
}
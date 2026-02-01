using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models;
using MotoMarket.Web.Models.ViewModels;
using MotoMarket.Web.Services;
using MotoMarket.Web.Services.Admin;
using MotoMarket.Web.Services.Listings;
using System.Diagnostics;

namespace MotoMarket.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IVehicleService _vehicleService;
        public HomeController(IAdminService adminService, IVehicleService vehicleService)
        {
            _adminService = adminService;
            _vehicleService = vehicleService;
        }
        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel();

            // 1. Pobieramy teksty z CMS (zróbmy try-catch ¿eby b³¹d CMS nie wywali³ strony g³ównej)
            try
            {
                var contents = await _adminService.GetAllPageContents();
                var header = contents.FirstOrDefault(x => x.PageKey == "HOMEPAGE_HEADER");
                var sub = contents.FirstOrDefault(x => x.PageKey == "HOMEPAGE_SUBTITLE");

                if (header != null) vm.WelcomeTitle = header.Content;
                if (sub != null) vm.WelcomeSubtitle = sub.Content;
            }
            catch { /* Ignorujemy, zostaj¹ domyœlne */ }

            // 2. Pobieramy marki do wyszukiwarki
            // Tutaj u¿yj metody, któr¹ masz (np. _adminService.GetAllBrands() lub z DictionaryService)
            vm.Brands = await _adminService.GetAllBrands();

            // 3. Pobieramy ostatnie og³oszenia (np. 8 sztuk)
            vm.RecentListings = await _vehicleService.GetRecentListings(8);

            return View(vm);
        }

        public IActionResult Banned()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            var contents = await _adminService.GetAllPageContents();
            return View(contents);
        }

        public IActionResult NotFound()
        {
            return View();
        }
    }
}
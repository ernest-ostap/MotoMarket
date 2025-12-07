using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Services;

namespace MotoMarket.Web.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IVehicleService _vehicleService;

        public ListingsController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        // GET: Listings
        public async Task<IActionResult> Index()
        {
            // Pobieramy auta z API
            var listings = await _vehicleService.GetAllListings();

            // Przekazujemy do widoku
            return View(listings);
        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var listing = await _vehicleService.GetListingDetail(id);

            if (listing == null)
            {
                return NotFound(); // Lub przekierowanie na stronę błędu
            }

            return View(listing);
        }
    }
}
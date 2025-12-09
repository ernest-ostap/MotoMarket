using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotoMarket.Web.Models.ViewModels;
using MotoMarket.Web.Services.Listings;

namespace MotoMarket.Web.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly IDictionaryService _dictionaryService; 

        public ListingsController(IVehicleService vehicleService, IDictionaryService dictionaryService)
        {
            _vehicleService = vehicleService;
            _dictionaryService = dictionaryService;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateListingViewModel();

            // pobieranie wszystkiego rownocześnie
            var brandsTask = _dictionaryService.GetBrands();
            var fuelsTask = _dictionaryService.GetFuelTypes();
            var gearboxesTask = _dictionaryService.GetGearboxTypes();
            var bodiesTask = _dictionaryService.GetBodyTypes();
            var drivesTask = _dictionaryService.GetDriveTypes();
            var categoriesTask = _dictionaryService.GetVehicleCategories();

            await Task.WhenAll(brandsTask, fuelsTask, gearboxesTask, bodiesTask, drivesTask, categoriesTask);

            // mapowanie na SelectListItem
            viewModel.Brands = brandsTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
            viewModel.FuelTypes = fuelsTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
            viewModel.GearboxTypes = gearboxesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
            viewModel.BodyTypes = bodiesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
            viewModel.DriveTypes = drivesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
            viewModel.VehicleCategories = categoriesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));

            return View(viewModel);
        }

        // POST: Listings/Create
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateListingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Pobieramy wszystko równolegle (tak jak w GET)
                var brandsTask = _dictionaryService.GetBrands();
                var fuelsTask = _dictionaryService.GetFuelTypes();
                var gearboxesTask = _dictionaryService.GetGearboxTypes();
                var bodiesTask = _dictionaryService.GetBodyTypes();
                var drivesTask = _dictionaryService.GetDriveTypes();
                var categoriesTask = _dictionaryService.GetVehicleCategories();

                await Task.WhenAll(brandsTask, fuelsTask, gearboxesTask, bodiesTask, drivesTask, categoriesTask);

                // Przypisujemy do modelu
                model.Brands = brandsTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
                model.FuelTypes = fuelsTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
                model.GearboxTypes = gearboxesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
                model.BodyTypes = bodiesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
                model.DriveTypes = drivesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
                model.VehicleCategories = categoriesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));

                // Dodatkowo: Modele (kaskada)
                if (model.BrandId > 0)
                {
                    var models = await _dictionaryService.GetModels(model.BrandId);
                    model.Models = models.Select(x => new SelectListItem(x.Name, x.Id));
                }

                return View(model);
            }

            // Jeśli walidacja OK, wysyłamy do API
            await _vehicleService.CreateListing(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetModelsJson(int brandId)
        {
            var models = await _dictionaryService.GetModels(brandId);
            return Json(models);
        }
    }
}
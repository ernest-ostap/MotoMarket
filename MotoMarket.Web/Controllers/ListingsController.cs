using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotoMarket.Web.Models.ViewModels;
using MotoMarket.Web.Services.Listings;

namespace MotoMarket.Web.Controllers
{
    public class ListingsController : Controller
    {
        // Lokalne stałe statusów, żeby nie referencjonować warstwy domenowej
        private const int StatusActive = 1;
        private const int StatusSold = 2;
        private const int StatusArchived = 3;

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
            await PopulateDictionaries(viewModel);
            return View(viewModel);
        }

        // POST: Listings/Create
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateListingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDictionaries(model);
                return View(model);
            }

            // Jeśli walidacja OK, wysyłamy do API
            await _vehicleService.CreateListing(model);

            return RedirectToAction(nameof(Index));
        }

        // GET: Listings/MyListings
        [Authorize]
        public async Task<IActionResult> MyListings()
        {
            var listings = await _vehicleService.GetMyListings();
            return View(listings);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // 1. Pobierz dane ogłoszenia
            var listing = await _vehicleService.GetListingDetail(id);
            if (listing == null) return NotFound();

            // 2. SECURITY CHECK (Ważne!)
            // Sprawdź, czy to ogłoszenie należy do zalogowanego użytkownika
            // (Możesz tu użyć User.FindFirst(ClaimTypes.NameIdentifier) albo w serwise, 
            // na razie załóżmy optymistycznie, że user kliknął ze swojej listy, 
            // ale profesjonalnie trzeba to sprawdzić).

            // 3. Mapowanie DTO -> ViewModel (Wypełniamy formularz danymi z bazy)
            var viewModel = new CreateListingViewModel
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                VIN = listing.VIN,
                ProductionYear = listing.ProductionYear,
                Mileage = listing.Mileage,
                LocationCity = listing.LocationCity,
                LocationRegion = listing.LocationRegion,

                // Ustawiamy wybrane wartości
                BrandId = listing.BrandId,
                ModelId = listing.ModelId,
                FuelTypeId = listing.FuelTypeId, // Tu uwaga: ListingDetailDto musi mieć te IDki (BrandId, FuelTypeId)!
                GearboxTypeId = listing.GearboxTypeId,
                BodyTypeId = listing.BodyTypeId,
                DriveTypeId = listing.DriveTypeId,
                VehicleCategoryId = listing.VehicleCategoryId
            };

            // 4. Pobierz słowniki (Brands, Fuels...) - tak jak w Create
            await PopulateDictionaries(viewModel);

            return View(viewModel);
        }

        [Authorize] // Pamiętaj o tym!
        [HttpPost]
        [ValidateAntiForgeryToken] // Dobra praktyka bezpieczeństwa dla formularzy
        public async Task<IActionResult> Edit(int id, CreateListingViewModel model)
        {
            // Security check
            if (id != model.Id) return BadRequest();

            // Walidacja
            if (!ModelState.IsValid)
            {
                await PopulateDictionaries(model);
                return View(model); 
            }

            // wysyłamy do API
            try
            {
                await _vehicleService.UpdateListing(model);
                return RedirectToAction(nameof(MyListings));
            }
            catch (Exception ex)
            {
                // Opcjonalnie: obsługa błędu, np. API nie działa
                ModelState.AddModelError("", "Wystąpił błąd podczas zapisywania zmian.");
                await PopulateDictionaries(model);
                return View(model);
            }
        }

        // POST: Listings/Delete/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken] // Zabezpieczenie formularza
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _vehicleService.UpdateListingStatus(id, StatusArchived);
                TempData["Success"] = "Ogłoszenie zostało zarchiwizowane.";
            }
            catch
            {
                TempData["Error"] = "Nie udało się zarchiwizować ogłoszenia.";
            }

            return RedirectToAction(nameof(MyListings)); // Wracamy do tabelki
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await _vehicleService.UpdateListingStatus(id, StatusActive);
                TempData["Success"] = "Ogłoszenie zostało przywrócone.";
            }
            catch
            {
                TempData["Error"] = "Nie udało się przywrócić ogłoszenia.";
            }

            return RedirectToAction(nameof(MyListings));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkSold(int id)
        {
            try
            {
                await _vehicleService.UpdateListingStatus(id, StatusSold);
                TempData["Success"] = "Ogłoszenie oznaczone jako sprzedane.";
            }
            catch
            {
                TempData["Error"] = "Nie udało się oznaczyć jako sprzedane.";
            }

            return RedirectToAction(nameof(MyListings));
        }

        [HttpGet]
        public async Task<JsonResult> GetModelsJson(int brandId)
        {
            var models = await _dictionaryService.GetModels(brandId);
            return Json(models);
        }

        private async Task PopulateDictionaries(CreateListingViewModel model)
        {
            var brandsTask = _dictionaryService.GetBrands();
            var fuelsTask = _dictionaryService.GetFuelTypes();
            var gearboxesTask = _dictionaryService.GetGearboxTypes();
            var bodiesTask = _dictionaryService.GetBodyTypes();
            var drivesTask = _dictionaryService.GetDriveTypes();
            var categoriesTask = _dictionaryService.GetVehicleCategories();

            var featuresTask = _dictionaryService.GetFeatures();
            var paramsTask = _dictionaryService.GetParameters();

            await Task.WhenAll(brandsTask, fuelsTask, gearboxesTask, bodiesTask, drivesTask, categoriesTask, featuresTask, paramsTask);

            model.Brands = brandsTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
            model.FuelTypes = fuelsTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
            model.GearboxTypes = gearboxesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
            model.BodyTypes = bodiesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
            model.DriveTypes = drivesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));
            model.VehicleCategories = categoriesTask.Result.Select(x => new SelectListItem(x.Name, x.Id));

            model.AvailableFeatures = featuresTask.Result;
            model.AvailableParameters = paramsTask.Result;

            // Dodatkowo: Modele (kaskada) - tylko jeśli BrandId > 0
            if (model.BrandId > 0)
            {
                var models = await _dictionaryService.GetModels(model.BrandId);
                model.Models = models.Select(x => new SelectListItem(x.Name, x.Id));
            }
        }
    }
}
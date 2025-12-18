using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models.DTOs; // Upewnij się, że masz tu DictionaryDto
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Tylko dla admina
    public class BrandsController : Controller
    {
        private readonly IAdminService _adminService;

        public BrandsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // 1. LISTA (Index)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Pobiera listę marek z serwisu
            var brands = await _adminService.GetAllBrands();
            return View(brands);
        }

        // 2. DODAWANIE (Create)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("", "Nazwa nie może być pusta");
                return View();
            }

            var success = await _adminService.CreateBrand(name);
            if (success)
            {
                TempData["Success"] = "Marka dodana!";
                return RedirectToAction(nameof(Index)); // Wraca do listy
            }

            ModelState.AddModelError("", "Błąd zapisu (API)");
            return View();
        }

        // 3. EDYCJA (Edit)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _adminService.GetBrand(id);
            if (brand == null) return NotFound();

            return View(brand); // Przekazujemy istniejącą markę do formularza
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DictionaryDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _adminService.UpdateBrand(model.Id, model.Name);
            if (success)
            {
                TempData["Success"] = "Zaktualizowano markę!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Błąd edycji (API)");
            return View(model);
        }

        // MotoMarket.Web/Controllers/BrandsController.cs

        [HttpPost]
        [Route("Brands/ToggleActive/{id}")] // <--- DODAJ TO. To wymusza, że ten adres MUSI działać.
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _adminService.ToggleBrandActive(id);
            return RedirectToAction(nameof(Index));
        }

        // 4. USUWANIE (Delete)
        // Tutaj robimy od razu akcję (bez widoku potwierdzenia dla szybkości)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adminService.DeleteBrand(id);
            if (success)
            {
                TempData["Success"] = "Usunięto markę.";
            }
            else
            {
                TempData["Error"] = "Nie można usunąć (może jest używana?).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VehicleCategoriesController : Controller
    {
        private readonly IAdminService _adminService;

        public VehicleCategoriesController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vehicleCategories = await _adminService.GetAllVehicleCategories();
            return View(vehicleCategories);
        }

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

            var success = await _adminService.CreateVehicleCategory(name);
            if (success)
            {
                TempData["Success"] = "Kategoria pojazdu dodana!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Błąd zapisu (API)");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vehicleCategory = await _adminService.GetVehicleCategory(id);
            if (vehicleCategory == null) return NotFound();

            return View(vehicleCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DictionaryDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _adminService.UpdateVehicleCategory(model.Id, model.Name);
            if (success)
            {
                TempData["Success"] = "Zaktualizowano kategorię pojazdu!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Błąd edycji (API)");
            return View(model);
        }

        [HttpPost]
        [Route("VehicleCategories/ToggleActive/{id}")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _adminService.ToggleVehicleCategoryActive(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adminService.DeleteVehicleCategory(id);
            if (success)
            {
                TempData["Success"] = "Usunięto kategorię pojazdu.";
            }
            else
            {
                TempData["Error"] = "Nie można usunąć (może jest używana?).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}


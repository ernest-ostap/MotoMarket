using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FuelTypesController : Controller
    {
        private readonly IAdminService _adminService;

        public FuelTypesController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var fuelTypes = await _adminService.GetAllFuelTypes();
            return View(fuelTypes);
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

            var success = await _adminService.CreateFuelType(name);
            if (success)
            {
                TempData["Success"] = "Typ paliwa dodany!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Błąd zapisu (API)");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var fuelType = await _adminService.GetFuelType(id);
            if (fuelType == null) return NotFound();

            return View(fuelType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DictionaryDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _adminService.UpdateFuelType(model.Id, model.Name);
            if (success)
            {
                TempData["Success"] = "Zaktualizowano typ paliwa!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Błąd edycji (API)");
            return View(model);
        }

        [HttpPost]
        [Route("FuelTypes/ToggleActive/{id}")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _adminService.ToggleFuelTypeActive(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adminService.DeleteFuelType(id);
            if (success)
            {
                TempData["Success"] = "Usunięto typ paliwa.";
            }
            else
            {
                TempData["Error"] = "Nie można usunąć (może jest używany?).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GearboxTypesController : Controller
    {
        private readonly IAdminService _adminService;

        public GearboxTypesController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var gearboxTypes = await _adminService.GetAllGearboxTypes();
            return View(gearboxTypes);
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

            var success = await _adminService.CreateGearboxType(name);
            if (success)
            {
                TempData["Success"] = "Typ skrzyni biegów dodany!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Błąd zapisu (API)");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var gearboxType = await _adminService.GetGearboxType(id);
            if (gearboxType == null) return NotFound();

            return View(gearboxType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DictionaryDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _adminService.UpdateGearboxType(model.Id, model.Name);
            if (success)
            {
                TempData["Success"] = "Zaktualizowano typ skrzyni biegów!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Błąd edycji (API)");
            return View(model);
        }

        [HttpPost]
        [Route("GearboxTypes/ToggleActive/{id}")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _adminService.ToggleGearboxTypeActive(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adminService.DeleteGearboxType(id);
            if (success)
            {
                TempData["Success"] = "Usunięto typ skrzyni biegów.";
            }
            else
            {
                TempData["Error"] = "Nie można usunąć (może jest używany?).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}


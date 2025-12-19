using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BodyTypesController : Controller
    {
        private readonly IAdminService _adminService;

        public BodyTypesController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bodyTypes = await _adminService.GetAllBodyTypes();
            return View(bodyTypes);
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

            var success = await _adminService.CreateBodyType(name);
            if (success)
            {
                TempData["Success"] = "Typ nadwozia dodany!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Błąd zapisu (API)");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var bodyType = await _adminService.GetBodyType(id);
            if (bodyType == null) return NotFound();

            return View(bodyType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DictionaryDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _adminService.UpdateBodyType(model.Id, model.Name);
            if (success)
            {
                TempData["Success"] = "Zaktualizowano typ nadwozia!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Błąd edycji (API)");
            return View(model);
        }

        [HttpPost]
        [Route("BodyTypes/ToggleActive/{id}")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _adminService.ToggleBodyTypeActive(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adminService.DeleteBodyType(id);
            if (success)
            {
                TempData["Success"] = "Usunięto typ nadwozia.";
            }
            else
            {
                TempData["Error"] = "Nie można usunąć (może jest używany?).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}


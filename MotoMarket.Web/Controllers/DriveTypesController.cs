using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DriveTypesController : Controller
    {
        private readonly IAdminService _adminService;

        public DriveTypesController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var driveTypes = await _adminService.GetAllDriveTypes();
            return View(driveTypes);
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

            var success = await _adminService.CreateDriveType(name);
            if (success)
            {
                TempData["Success"] = "Typ napędu dodany!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Błąd zapisu (API)");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var driveType = await _adminService.GetDriveType(id);
            if (driveType == null) return NotFound();

            return View(driveType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DictionaryDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _adminService.UpdateDriveType(model.Id, model.Name);
            if (success)
            {
                TempData["Success"] = "Zaktualizowano typ napędu!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Błąd edycji (API)");
            return View(model);
        }

        [HttpPost]
        [Route("DriveTypes/ToggleActive/{id}")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _adminService.ToggleDriveTypeActive(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adminService.DeleteDriveType(id);
            if (success)
            {
                TempData["Success"] = "Usunięto typ napędu.";
            }
            else
            {
                TempData["Error"] = "Nie można usunąć (może jest używany?).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}


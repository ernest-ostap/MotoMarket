using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ParameterTypesController : Controller
    {
        private readonly IAdminService _adminService;
        public ParameterTypesController(IAdminService adminService) => _adminService = adminService;

        public async Task<IActionResult> Index() => View(await _adminService.GetAllParameterTypes());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string name, string? unit, string inputType, string category, bool isRequired = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("name", "Nazwa jest wymagana");
                return View();
            }

            if (string.IsNullOrWhiteSpace(inputType))
            {
                ModelState.AddModelError("inputType", "Typ wejścia jest wymagany");
                return View();
            }

            if (string.IsNullOrWhiteSpace(category))
            {
                ModelState.AddModelError("category", "Kategoria jest wymagana");
                return View();
            }

            // Jednostka może być pusta (np. dla "liczba drzwi")
            unit = string.IsNullOrWhiteSpace(unit) ? string.Empty : unit;

            var result = await _adminService.CreateParameterType(name, unit ?? string.Empty, inputType, category, isRequired);
            if (result)
            {
                TempData["Success"] = "Typ parametru dodany!";
                return RedirectToAction(nameof(Index));
            }
            
            ModelState.AddModelError("", "Błąd zapisu (API) - sprawdź czy wszystkie pola są poprawnie wypełnione");
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var parameterType = await _adminService.GetParameterType(id);
            if (parameterType == null) return NotFound();
            return View(parameterType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ParameterTypeDto model)
        {
            if (await _adminService.UpdateParameterType(model.Id, model.Name, model.Unit, model.InputType, model.Category, model.IsRequired))
            {
                TempData["Success"] = "Zaktualizowano typ parametru!";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Błąd edycji (API)");
            return View(model);
        }

        [HttpPost]
        [Route("ParameterTypes/ToggleActive/{id}")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _adminService.ToggleParameterTypeActive(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adminService.DeleteParameterType(id);
            if (success)
            {
                TempData["Success"] = "Usunięto typ parametru.";
            }
            else
            {
                TempData["Error"] = "Nie można usunąć (może jest używany?).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}


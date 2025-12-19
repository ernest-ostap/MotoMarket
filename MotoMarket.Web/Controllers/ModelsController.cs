using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ModelsController : Controller
    {
        private readonly IAdminService _adminService;

        public ModelsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var models = await _adminService.GetAllModels();
            return View(models);
        }

        // CREATE
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Musimy załadować marki do dropdowna
            ViewBag.Brands = await _adminService.GetAllBrands();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, int brandId)
        {
            if (await _adminService.CreateModel(name, brandId))
            {
                TempData["Success"] = "Model dodany!";
                return RedirectToAction(nameof(Index));
            }

            // Jak błąd, ładujemy marki ponownie
            ViewBag.Brands = await _adminService.GetAllBrands();
            return View();
        }

        // EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _adminService.GetModel(id);
            if (model == null) return NotFound();

            ViewBag.Brands = await _adminService.GetAllBrands();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ModelDto model)
        {
            if (await _adminService.UpdateModel(model.Id, model.Name, model.BrandId))
            {
                TempData["Success"] = "Zaktualizowano model!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Brands = await _adminService.GetAllBrands();
            return View(model);
        }

        [HttpPost]
        [Route("Models/ToggleActive/{id}")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _adminService.ToggleModelActive(id);
            return RedirectToAction(nameof(Index));
        }

        // 4. USUWANIE (Delete)
        // Tutaj robimy od razu akcję (bez widoku potwierdzenia dla szybkości)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adminService.DeleteModel(id);
            if (success)
            {
                TempData["Success"] = "Usunięto model.";
            }
            else
            {
                TempData["Error"] = "Nie można usunąć (może jest używana?).";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
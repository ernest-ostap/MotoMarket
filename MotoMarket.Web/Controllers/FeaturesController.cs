using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FeaturesController : Controller
    {
        private readonly IAdminService _adminService;
        public FeaturesController(IAdminService adminService) => _adminService = adminService;

        public async Task<IActionResult> Index() => View(await _adminService.GetAllFeatures());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string name, string groupName)
        {
            if (await _adminService.CreateFeature(name, groupName)) return RedirectToAction(nameof(Index));
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var feature = await _adminService.GetFeature(id);
            if (feature == null) return NotFound();
            return View(feature);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FeatureDto model)
        {
            if (await _adminService.UpdateFeature(model.Id, model.Name, model.GroupName)) return RedirectToAction(nameof(Index));
            return View(model);
        }

        [HttpPost]
        [Route("Features/ToggleActive/{id}")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _adminService.ToggleFeatureActive(id);
            return RedirectToAction(nameof(Index));
        }

        // POST [id] delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adminService.DeleteFeature(id);
            if (success)
            {
                TempData["Success"] = "Usunięto element.";
            }
            else
            {
                TempData["Error"] = "Nie można usunąć (może jest używana?).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
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
        public async Task<IActionResult> Create(ParameterTypeDto model)
        {
            if (await _adminService.CreateParameterType(model)) return RedirectToAction(nameof(Index));
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _adminService.GetParameterType(id); // Dorób to w serwisie
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ParameterTypeDto model)
        {
            if (await _adminService.UpdateParameterType(model)) return RedirectToAction(nameof(Index));
            return View(model);
        }

        [HttpPost]
        [Route("ParameterTypes/ToggleActive/{id}")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _adminService.ToggleParameterTypeActive(id);
            return RedirectToAction(nameof(Index));
        }

        // POST [id] delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adminService.DeleteParameterType(id);
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

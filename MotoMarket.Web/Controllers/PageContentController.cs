using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PageContentController : Controller
    {
        private readonly IAdminService _adminService;
        public PageContentController(IAdminService adminService) => _adminService = adminService;

        // Lista tekstów do edycji
        public async Task<IActionResult> Index()
        {
            var contents = await _adminService.GetAllPageContents();
            return View(contents);
        }

        // Formularz edycji
        public async Task<IActionResult> Edit(int id)
        {
            var contents = await _adminService.GetAllPageContents();
            var item = contents.FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string content)
        {
            await _adminService.UpdatePageContent(id, content);
            return RedirectToAction(nameof(Index));
        }
    }
}

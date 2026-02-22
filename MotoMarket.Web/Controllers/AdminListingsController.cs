using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminListingsController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminListingsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var listings = await _adminService.GetAllListings();
            return View(listings);
        }

        // GET [id] ban form
        [HttpGet]
        public async Task<IActionResult> Ban(int id)
        {
            return View(id);
        }

        [HttpPost]
        public async Task<IActionResult> Unban(int id)
        {
            await _adminService.UnbanListing(id);
            TempData["Success"] = "Ogłoszenie przywrócone (odbanowane).";
            return RedirectToAction(nameof(Index));
        }

        // POST [id] ban confirm
        [HttpPost]
        public async Task<IActionResult> BanConfirm(int id, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                ModelState.AddModelError("", "Musisz podać powód!");
                return View("Ban", id);
            }

            await _adminService.BanListing(id, reason);
            TempData["Success"] = "Ogłoszenie zbanowane.";
            return RedirectToAction(nameof(Index));
        }
    }
}

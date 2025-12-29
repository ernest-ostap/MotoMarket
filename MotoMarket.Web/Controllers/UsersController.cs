using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Services.Admin;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")] // BARDZO WAŻNE
    public class UsersController : Controller
    {
        private readonly IAdminService _adminService;

        public UsersController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _adminService.GetAllUsers();
            return View(users);
        }

        [HttpPost]
        [Route("Users/ToggleBan/{id}")] // Wymuszamy trasę, żeby nie było 404
        public async Task<IActionResult> ToggleBan(string id)
        {
            await _adminService.ToggleUserBan(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
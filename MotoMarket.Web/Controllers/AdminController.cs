using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MotoMarket.Web.Controllers
{
    [Authorize(Roles = "Admin")] // <--- TYLKO ADMIN MA WSTĘP!
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
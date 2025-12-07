using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models;
using MotoMarket.Web.Services;
using System.Diagnostics;

namespace MotoMarket.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // ... Error() itp.
    }
}
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Web.Models.ViewModels;
using MotoMarket.Web.Services.Auth;

namespace MotoMarket.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var token = await _authService.Login(model);

            if (!string.IsNullOrEmpty(token))
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("JWT", token, cookieOptions);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Nieprawidłowy email lub hasło");
            return View(model);
        }

        // GET
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _authService.Register(model);
            if (success)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Błąd rejestracji (email zajęty?)");
            return View(model);
        }

        //Logging out and clears the JWT cookie and redirects to the home page.
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();
            Response.Cookies.Delete("JWT");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyData()
        {
            var profile = await _authService.GetUserProfile();
            if (profile == null) return NotFound();
            return Json(profile);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Nieprawidłowe dane.";
                return RedirectToAction("Index", "Home");
            }

            var success = await _authService.UpdateProfile(model);
            TempData[success ? "Success" : "Error"] = success ? "Dane zaktualizowane." : "Błąd aktualizacji danych.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _authService.ChangePassword(model);
            if (success)
            {
                TempData["Success"] = "Hasło zostało zmienione.";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Błąd zmiany hasła. Sprawdź, czy obecne hasło jest poprawne.");
            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
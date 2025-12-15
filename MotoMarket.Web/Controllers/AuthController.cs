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

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // ZMIANA: Oczekujemy, że serwis zwróci stringa (token) lub null/empty
            // Uwaga: Musisz zaktualizować AuthService (patrz niżej)!
            var token = await _authService.Login(model);

            if (!string.IsNullOrEmpty(token))
            {
                // --- NOWOŚĆ: Zapisujemy token w ciasteczku ---
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // Bezpieczne (JS nie ukradnie, ale serwer odczyta)
                    Secure = true,   // Tylko HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("JWT", token, cookieOptions);
                // ---------------------------------------------

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Nieprawidłowy email lub hasło");
            return View(model);
        }

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
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

        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();

            // --- NOWOŚĆ: Czyścimy ciasteczko przy wylogowaniu ---
            Response.Cookies.Delete("JWT");
            // ----------------------------------------------------

            return RedirectToAction("Index", "Home");
        }

        // ... Reszta metod (UpdateProfile, ChangePassword) bez zmian ...
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

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Nieprawidłowe dane hasła.";
                return RedirectToAction("Index", "Home");
            }

            var success = await _authService.ChangePassword(model);
            TempData[success ? "Success" : "Error"] = success ? "Hasło zostało zmienione." : "Błąd zmiany hasła.";
            return RedirectToAction("Index", "Home");
        }
    }
}
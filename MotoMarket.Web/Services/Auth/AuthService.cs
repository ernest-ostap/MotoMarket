using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using MotoMarket.Web.Models.DTOs; // Tu musi być Twój AuthDto (skopiowany z API lub stworzony)
using MotoMarket.Web.Models.ViewModels;
using System.Net.Http.Headers;

namespace MotoMarket.Web.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor; // To pozwala operować na ciasteczkach w serwisie

        public AuthService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiUrl"] ?? "https://localhost:7072";
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Login(LoginViewModel model)
        {
            // 1. Wysyłamy login/hasło do API
            var loginJson = JsonSerializer.Serialize(model);
            var content = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Users/login", content);

            if (!response.IsSuccessStatusCode)
                return false;

            // 2. Odbieramy Token
            var responseString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var authResult = JsonSerializer.Deserialize<AuthDto>(responseString, options);

            if (authResult == null || string.IsNullOrEmpty(authResult.Token))
                return false;

            // 3. Budujemy Ciasteczko dla MVC (Logujemy usera w przeglądarce)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, authResult.Email),
                new Claim(ClaimTypes.NameIdentifier, authResult.Id),
                new Claim("JWT", authResult.Token), // ZAPISUJEMY TOKEN W CIASTKU! Ważne na przyszłość.
                new Claim("FirstName", authResult.FirstName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // "Zapamiętaj mnie"
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            };

            // Fizyczne zalogowanie w MVC
            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return true;
        }

        public async Task<bool> Register(RegisterViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Users/register", content);

            if (!response.IsSuccessStatusCode) return false;

            // Po rejestracji API zwraca też token, więc od razu logujemy
            // (Można tu powtórzyć logikę z Login, ale dla uproszczenia załóżmy, że user musi się zalogować po rejestracji 
            // LUB skopiuj logikę SignInAsync tutaj też, jeśli API zwraca AuthDto).

            return true;
        }

        public async Task Logout()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<bool> UpdateProfile(UpdateProfileViewModel model)
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Users/update-profile", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ChangePassword(ChangePasswordViewModel model)
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Users/change-password", content);
            return response.IsSuccessStatusCode;
        }
    }
}
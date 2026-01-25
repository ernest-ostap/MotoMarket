using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Models.ViewModels;
using System.Net.Http.Headers;
// DODAJ TE DWA USINGI (po zainstalowaniu paczki przestaną być szare/czerwone):
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace MotoMarket.Web.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiUrl"] ?? "https://localhost:7072";
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string?> Login(LoginViewModel model)
        {
            // 1. Wysyłamy login/hasło do API
            var loginJson = JsonSerializer.Serialize(model);
            var content = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Users/login", content);

            if (!response.IsSuccessStatusCode)
                return null;

            // 2. Odbieramy Token
            var responseString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var authResult = JsonSerializer.Deserialize<AuthDto>(responseString, options);

            if (authResult == null || string.IsNullOrEmpty(authResult.Token))
                return null;

            // --- PARSOWANIE RÓL Z TOKENA ---
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(authResult.Token);

            // Wyciągamy role (szukamy pod różnymi nazwami, bo JWT bywa wredny)
            var roleClaims = jwtToken.Claims
                .Where(c => c.Type == "role" || c.Type == "roles" || c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            // -------------------------------

            // 3. Budujemy listę Claims dla Ciasteczka
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, authResult.Email),
                new Claim(ClaimTypes.NameIdentifier, authResult.Id),
                new Claim("JWT", authResult.Token), // Token do SignalR
                new Claim("FirstName", authResult.FirstName)
            };

            // Dodajemy role do ciasteczka jako poprawny typ
            foreach (var role in roleClaims)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            };

            // Fizyczne zalogowanie w przeglądarce
            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // 4. Zwracamy token (dla AuthController)
            return authResult.Token;
        }

        
        public async Task<bool> Register(RegisterViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Users/register", content);
            return response.IsSuccessStatusCode;
        }

        public async Task Logout()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<UpdateProfileViewModel> GetUserProfile()
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Users/profile");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<UpdateProfileViewModel>(json, options);
            }

            return null;
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

        public async Task<bool> IsUserBanned(string userId)
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Users/{userId}/is-banned");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                // API zwraca true/false
                bool.TryParse(json, out var isBanned);
                return isBanned;
            }

            return false; // Jak błąd API, to na wszelki wypadek nie banujmy (fail-open)
        }
    }
}
using MotoMarket.Web.Models.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MotoMarket.Web.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiUrl"] ?? "https://localhost:7072";
            _httpContextAccessor = httpContextAccessor;
        }

        #region Brands
        public async Task<bool> CreateBrand(string name)
        {
            // 1. Pobierz token JWT (żeby API wiedziało, że jesteś Adminem)
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 2. Przygotuj dane do wysyłki
            // Tworzymy anonimowy obiekt, który pasuje do CreateBrandCommand w API
            var command = new { Name = name };
            var json = JsonSerializer.Serialize(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 3. Wyślij POST do API
            // Zakładam, że w BrandsController (API) masz [HttpPost] w /api/Brands
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Brands", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<DictionaryDto>> GetAllBrands()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Brands");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<DictionaryDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<DictionaryDto>();
            }
            return new List<DictionaryDto>();
        }

        public async Task<DictionaryDto?> GetBrand(int id)
        {
            var brands = await GetAllBrands();
            return brands.FirstOrDefault(b => b.Id == id);
        }

        public async Task<bool> UpdateBrand(int id, string name)
        {
            AddAuthHeader(); // Twoja metoda dodająca token
            var json = JsonSerializer.Serialize(new { Id = id, Name = name });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/Brands/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ToggleBrandActive(int id)
        {
            AddAuthHeader();
            // Wysyłamy pusty content, bo ID jest w URL
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"{_apiBaseUrl}/api/Brands/{id}/toggle-active", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBrand(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/Brands/{id}");
            return response.IsSuccessStatusCode;
        }
        #endregion

        #region Models

        #endregion

        #region Models

        #endregion

        #region Models

        #endregion

        #region Models

        #endregion

        #region Models

        #endregion

        #region Models

        #endregion


        private void AddAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
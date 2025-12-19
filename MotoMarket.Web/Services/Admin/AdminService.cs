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

        #region DriveTypes
        public async Task<IEnumerable<DictionaryDto>> GetAllDriveTypes()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/DriveTypes");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<DictionaryDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<DictionaryDto>();
            }
            return new List<DictionaryDto>();
        }

        public async Task<DictionaryDto?> GetDriveType(int id)
        {
            var driveTypes = await GetAllDriveTypes();
            return driveTypes.FirstOrDefault(d => d.Id == id);
        }

        public async Task<bool> CreateDriveType(string name)
        {
            AddAuthHeader();
            var command = new { Name = name };
            var json = JsonSerializer.Serialize(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/DriveTypes", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateDriveType(int id, string name)
        {
            AddAuthHeader();
            var json = JsonSerializer.Serialize(new { Id = id, Name = name });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/DriveTypes/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ToggleDriveTypeActive(int id)
        {
            AddAuthHeader();
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"{_apiBaseUrl}/api/DriveTypes/{id}/toggle-active", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDriveType(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/DriveTypes/{id}");
            return response.IsSuccessStatusCode;
        }
        #endregion

        #region BodyTypes
        public async Task<IEnumerable<DictionaryDto>> GetAllBodyTypes()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/BodyTypes");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<DictionaryDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<DictionaryDto>();
            }
            return new List<DictionaryDto>();
        }

        public async Task<DictionaryDto?> GetBodyType(int id)
        {
            var bodyTypes = await GetAllBodyTypes();
            return bodyTypes.FirstOrDefault(b => b.Id == id);
        }

        public async Task<bool> CreateBodyType(string name)
        {
            AddAuthHeader();
            var command = new { Name = name };
            var json = JsonSerializer.Serialize(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/BodyTypes", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateBodyType(int id, string name)
        {
            AddAuthHeader();
            var json = JsonSerializer.Serialize(new { Id = id, Name = name });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/BodyTypes/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ToggleBodyTypeActive(int id)
        {
            AddAuthHeader();
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"{_apiBaseUrl}/api/BodyTypes/{id}/toggle-active", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBodyType(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/BodyTypes/{id}");
            return response.IsSuccessStatusCode;
        }
        #endregion

        #region FuelTypes
        public async Task<IEnumerable<DictionaryDto>> GetAllFuelTypes()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/FuelTypes");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<DictionaryDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<DictionaryDto>();
            }
            return new List<DictionaryDto>();
        }

        public async Task<DictionaryDto?> GetFuelType(int id)
        {
            var fuelTypes = await GetAllFuelTypes();
            return fuelTypes.FirstOrDefault(f => f.Id == id);
        }

        public async Task<bool> CreateFuelType(string name)
        {
            AddAuthHeader();
            var command = new { Name = name };
            var json = JsonSerializer.Serialize(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/FuelTypes", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateFuelType(int id, string name)
        {
            AddAuthHeader();
            var json = JsonSerializer.Serialize(new { Id = id, Name = name });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/FuelTypes/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ToggleFuelTypeActive(int id)
        {
            AddAuthHeader();
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"{_apiBaseUrl}/api/FuelTypes/{id}/toggle-active", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteFuelType(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/FuelTypes/{id}");
            return response.IsSuccessStatusCode;
        }
        #endregion

        #region GearboxTypes
        public async Task<IEnumerable<DictionaryDto>> GetAllGearboxTypes()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/GearboxTypes");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<DictionaryDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<DictionaryDto>();
            }
            return new List<DictionaryDto>();
        }

        public async Task<DictionaryDto?> GetGearboxType(int id)
        {
            var gearboxTypes = await GetAllGearboxTypes();
            return gearboxTypes.FirstOrDefault(g => g.Id == id);
        }

        public async Task<bool> CreateGearboxType(string name)
        {
            AddAuthHeader();
            var command = new { Name = name };
            var json = JsonSerializer.Serialize(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/GearboxTypes", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateGearboxType(int id, string name)
        {
            AddAuthHeader();
            var json = JsonSerializer.Serialize(new { Id = id, Name = name });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/GearboxTypes/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ToggleGearboxTypeActive(int id)
        {
            AddAuthHeader();
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"{_apiBaseUrl}/api/GearboxTypes/{id}/toggle-active", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteGearboxType(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/GearboxTypes/{id}");
            return response.IsSuccessStatusCode;
        }
        #endregion

        #region VehicleCategories
        public async Task<IEnumerable<DictionaryDto>> GetAllVehicleCategories()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/VehicleCategories");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<DictionaryDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<DictionaryDto>();
            }
            return new List<DictionaryDto>();
        }

        public async Task<DictionaryDto?> GetVehicleCategory(int id)
        {
            var vehicleCategories = await GetAllVehicleCategories();
            return vehicleCategories.FirstOrDefault(v => v.Id == id);
        }

        public async Task<bool> CreateVehicleCategory(string name)
        {
            AddAuthHeader();
            var command = new { Name = name };
            var json = JsonSerializer.Serialize(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/VehicleCategories", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateVehicleCategory(int id, string name)
        {
            AddAuthHeader();
            var json = JsonSerializer.Serialize(new { Id = id, Name = name });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/VehicleCategories/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ToggleVehicleCategoryActive(int id)
        {
            AddAuthHeader();
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"{_apiBaseUrl}/api/VehicleCategories/{id}/toggle-active", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteVehicleCategory(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/VehicleCategories/{id}");
            return response.IsSuccessStatusCode;
        }
        #endregion

        #region Features

        #endregion

        #region ParametersTypes

        #endregion

        #region VehicleParameters

        #endregion

        private void AddAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
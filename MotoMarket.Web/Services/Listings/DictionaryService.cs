using MotoMarket.Web.Models.DTOs;
using System.Text.Json;

namespace MotoMarket.Web.Services.Listings
{
    public class DictionaryService : IDictionaryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public DictionaryService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiUrl"] ?? "https://localhost:7072";
        }

        public async Task<IEnumerable<SelectListItemDto>> GetBrands()
        {
            return await GetList("/api/Dictionaries/brands");
        }

        public async Task<IEnumerable<SelectListItemDto>> GetModels(int brandId)
        {
            return await GetList($"/api/Dictionaries/models/{brandId}");
        }

        public async Task<IEnumerable<SelectListItemDto>> GetBodyTypes()
        {
            return await GetList("/api/Dictionaries/body-types");
        }
        public async Task<IEnumerable<SelectListItemDto>> GetDriveTypes()
        {
            return await GetList("/api/Dictionaries/drive-types");
        }
        public async Task<IEnumerable<SelectListItemDto>> GetFuelTypes()
        {
            return await GetList("/api/Dictionaries/fuel-types");
        }
        public async Task<IEnumerable<SelectListItemDto>> GetGearboxTypes()
        {
            return await GetList("/api/Dictionaries/gearbox-types");
        }
        public async Task<IEnumerable<SelectListItemDto>> GetVehicleCategories()
        {
            return await GetList("/api/Dictionaries/vehicle-categories");
        }

        private async Task<IEnumerable<SelectListItemDto>> GetList(string endpoint)
        {
            var response = await _httpClient.GetAsync(_apiBaseUrl + endpoint);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var list = JsonSerializer.Deserialize<IEnumerable<dynamic>>(json, options);

                var result = new List<SelectListItemDto>();
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        result.Add(new SelectListItemDto
                        {
                            Id = element.GetProperty("id").GetInt32().ToString(),
                            Name = element.GetProperty("name").GetString()!
                        });
                    }
                }
                return result;
            }
            return new List<SelectListItemDto>();
        }

        public async Task<IEnumerable<FeatureDto>> GetFeatures()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Dictionaries/features");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<IEnumerable<FeatureDto>>(json, options) ?? new List<FeatureDto>();
            }
            return new List<FeatureDto>();
        }

        public async Task<IEnumerable<ParameterTypeDto>> GetParameters()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Dictionaries/parameters");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<IEnumerable<ParameterTypeDto>>(json, options) ?? new List<ParameterTypeDto>();
            }
            return new List<ParameterTypeDto>();
        }
    }
}

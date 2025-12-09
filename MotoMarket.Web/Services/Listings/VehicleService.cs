using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Models.ViewModels;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MotoMarket.Web.Services.Listings
{
    public class VehicleService : IVehicleService
    {
        #region Pola i konstruktor
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VehicleService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiUrl"] ?? "https://localhost:7072";
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region GetAllListings
        public async Task<IEnumerable<ListingDto>> GetAllListings()
        {
            // Wywołujemy GET /api/Listings
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Listings");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Opcje deserializacji (żeby wielkość liter nie miała znaczenia: id vs Id)
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<IEnumerable<ListingDto>>(json, options) ?? new List<ListingDto>();
            }

            return new List<ListingDto>(); // Lub obsługa błędów
        }

        public async Task<ListingDetailDto?> GetListingDetail(int id)
        {
            // Strzelamy do API: /api/Listings/5
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Listings/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<ListingDetailDto>(json, options);
            }

            return null; // Jeśli API zwróci 404 lub błąd
        }

        public async Task<IEnumerable<ListingDto>> GetMyListings()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value);

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Listings/mine");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<IEnumerable<ListingDto>>(json, options) ?? new List<ListingDto>();
            }
            return new List<ListingDto>();
        }
        #endregion

        public async Task CreateListing(CreateListingViewModel model)
        {
            var command = new
            {
                model.Title,
                model.Description,
                model.Price,
                model.BrandId,
                model.ModelId,
                model.FuelTypeId,
                model.GearboxTypeId,
                model.DriveTypeId,
                model.BodyTypeId,
                model.VehicleCategoryId,
                model.VIN,
                model.ProductionYear,
                model.Mileage,
                model.LocationCity,
                model.LocationRegion,
            };

            // Serializacja i wysyłka
            var json = JsonSerializer.Serialize(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // doklejenie tokena JWT do nagłówka Authorization
            // token bierzemy z ciasteczka 
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            // 3. Wysyłamy
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Listings", content);

            // 4. Rzucamy błąd, jeśli API zwróci np. 400 lub 500
            response.EnsureSuccessStatusCode();
        }
        

        public async Task DeleteListing(int id)
        {
            // Doklejamy token (ważne, bo API sprawdzi czy to Twoje ogłoszenie... 
            // chociaż API delete na razie nie sprawdza ownera, ale [Authorize] jest wymagane)
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value);

            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/Listings/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
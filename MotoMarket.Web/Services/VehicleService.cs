using MotoMarket.Web.Models.DTOs;
using System.Text.Json; 

namespace MotoMarket.Web.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public VehicleService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Pobierzemy adres API z appsettings.json
            _apiBaseUrl = configuration["ApiUrl"] ?? "https://localhost:7072";
        }

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
    }
}
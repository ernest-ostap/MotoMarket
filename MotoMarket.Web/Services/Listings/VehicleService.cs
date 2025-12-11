using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Models.ViewModels;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.IO;

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
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; //deserializacja nie zważająca na wielkość liter
                var listings = JsonSerializer.Deserialize<IEnumerable<ListingDto>>(json, options) ?? new List<ListingDto>();

                foreach (var item in listings)
                {
                    if (!string.IsNullOrEmpty(item.MainPhotoUrl))
                    {
                        // Doklejamy adres API (np. https://localhost:7072) do ścieżki (/uploads/...)
                        item.MainPhotoUrl = _apiBaseUrl + item.MainPhotoUrl;
                    }
                }
                return listings;
            }

            return new List<ListingDto>(); // Lub obsługa błędów
        }

        public async Task<ListingDetailDto?> GetListingDetail(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Listings/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var listing = JsonSerializer.Deserialize<ListingDetailDto>(json, options);

                if (listing != null)
                {
                    // --- POPRAWKA GŁÓWNEGO ZDJĘCIA ---
                    if (!string.IsNullOrEmpty(listing.MainPhotoUrl))
                    {
                        listing.MainPhotoUrl = _apiBaseUrl + listing.MainPhotoUrl;
                    }

                    // --- POPRAWKA GALERII (Jeśli masz listę URL-i) ---
                    if (listing.PhotoUrls != null)
                    {
                        // Tworzymy nową listę z poprawionymi adresami
                        listing.PhotoUrls = listing.PhotoUrls
                            .Select(url => _apiBaseUrl + url)
                            .ToList();
                    }
                }

                return listing;
            }
            return null;
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
                var listings = JsonSerializer.Deserialize<IEnumerable<ListingDto>>(json, options) ?? new List<ListingDto>();

                // --- POPRAWKA URL ---
                foreach (var item in listings)
                {
                    if (!string.IsNullOrEmpty(item.MainPhotoUrl))
                    {
                        // Doklejamy adres API (np. https://localhost:7072) do ścieżki (/uploads/...)
                        item.MainPhotoUrl = _apiBaseUrl + item.MainPhotoUrl;
                    }
                }
                // --------------------

                return listings;
            }
            return new List<ListingDto>();
        }
        #endregion

        public async Task CreateListing(CreateListingViewModel model)
        {
            // Używamy MultipartFormDataContent zamiast JSON
            using var content = new MultipartFormDataContent();

            // Dodajemy zwykłe pola tekstowe
            content.Add(new StringContent(model.Title ?? ""), "Title");
            content.Add(new StringContent(model.Description ?? ""), "Description");
            content.Add(new StringContent(model.Price.ToString()), "Price");
            content.Add(new StringContent(model.BrandId.ToString()), "BrandId");
            content.Add(new StringContent(model.ModelId.ToString()), "ModelId");
            content.Add(new StringContent(model.VehicleCategoryId.ToString()), "VehicleCategoryId");
            content.Add(new StringContent(model.FuelTypeId.ToString()), "FuelTypeId");
            content.Add(new StringContent(model.GearboxTypeId.ToString()), "GearboxTypeId");
            content.Add(new StringContent(model.DriveTypeId.ToString()), "DriveTypeId");
            content.Add(new StringContent(model.BodyTypeId.ToString()), "BodyTypeId");
            content.Add(new StringContent(model.VIN ?? ""), "VIN");
            content.Add(new StringContent(model.ProductionYear.ToString()), "ProductionYear");
            content.Add(new StringContent(model.Mileage.ToString()), "Mileage");
            content.Add(new StringContent(model.LocationCity ?? ""), "LocationCity");
            content.Add(new StringContent(model.LocationRegion ?? ""), "LocationRegion");

            // Dodajemy PLIKI
            if (model.Photos != null)
            {
                foreach (var file in model.Photos)
                {
                    // Konwertujemy plik na strumień i dodajemy do requestu
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                    // "Photos" to musi być ta sama nazwa co w API (CreateListingApiRequest.Photos)
                    content.Add(fileContent, "Photos", file.FileName);
                }
            }

            // Dodajemy Token JWT
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Wysyłamy!
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Listings", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task UpdateListing(CreateListingViewModel model)
        {
            // Budujemy obiekt zgodny z UpdateListingCommand w API
            var command = new
            {
                Id = model.Id, // <--- Ważne!
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,

                BrandId = model.BrandId,
                ModelId = model.ModelId,
                VehicleCategoryId = model.VehicleCategoryId,
                FuelTypeId = model.FuelTypeId,
                GearboxTypeId = model.GearboxTypeId,
                DriveTypeId = model.DriveTypeId,
                BodyTypeId = model.BodyTypeId,

                VIN = model.VIN,
                ProductionYear = model.ProductionYear,
                Mileage = model.Mileage,
                LocationCity = model.LocationCity,
                LocationRegion = model.LocationRegion
            };

            var json = JsonSerializer.Serialize(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Token
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // PUT
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/Listings/{model.Id}", content);
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

        public async Task RestoreListing(int id)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value);

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Listings/{id}/restore", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateListingStatus(int id, int status)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value);

            var payload = new { Id = id, Status = status };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Listings/{id}/status", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
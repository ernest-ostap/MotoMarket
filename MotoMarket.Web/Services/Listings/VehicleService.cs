using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Models.ViewModels;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

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

        #region Get...Listings
        public async Task<IEnumerable<ListingDto>> GetAllListings(ListingsFilterViewModel filter)
        {
            // Budowanie Query String
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(filter.Search)) queryParams.Add($"searchCallback={filter.Search}");
            if (filter.BrandId.HasValue) queryParams.Add($"brandId={filter.BrandId}");
            if (filter.ModelId.HasValue) queryParams.Add($"modelId={filter.ModelId}");
            if (filter.PriceMin.HasValue) queryParams.Add($"priceMin={filter.PriceMin}");
            if (filter.PriceMax.HasValue) queryParams.Add($"priceMax={filter.PriceMax}");
            if (filter.YearMin.HasValue) queryParams.Add($"yearMin={filter.YearMin}");
            if (!string.IsNullOrEmpty(filter.SortBy)) queryParams.Add($"sortBy={filter.SortBy}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

            // Dodajemy token autoryzacyjny (jeśli użytkownik jest zalogowany)
            // API użyje tego do sprawdzenia ulubionych
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Listings{queryString}");

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
            // Dodajemy token autoryzacyjny (jeśli użytkownik jest zalogowany)
            // API użyje tego do sprawdzenia ulubionych
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

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

        #region Favorites
        public async Task<bool> ToggleFavorite(int listingId)
        {
            // Token
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Strzał do API endpointa POST /api/Favorites/{id}
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/Favorites/{listingId}", null);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                // API zwraca true/false
                return bool.Parse(json);
            }

            return false;
        }

        public async Task<IEnumerable<ListingDto>> GetMyFavorites()
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Favorites");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var listings = JsonSerializer.Deserialize<IEnumerable<ListingDto>>(json, options) ?? new List<ListingDto>();

                // Poprawka URL zdjęć (skopiuj tę pętlę z GetAllListings)
                foreach (var item in listings)
                {
                    if (!string.IsNullOrEmpty(item.MainPhotoUrl)) item.MainPhotoUrl = _apiBaseUrl + item.MainPhotoUrl;
                }

                return listings;
            }
            return new List<ListingDto>();
        }
        #endregion

        #region Create
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

            // Metadane zdjęć
            content.Add(new StringContent(model.MainPhotoIndex.ToString()), "MainPhotoIndex");

            // Dodajemy PLIKI
            if (model.Photos != null)
            {
                var photoList = model.Photos.ToList();
                for (int i = 0; i < photoList.Count; i++)
                {
                    var file = photoList[i];
                    // Konwertujemy plik na strumień i dodajemy do requestu
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                    // "Photos" to musi być ta sama nazwa co w API (CreateListingApiRequest.Photos)
                    content.Add(fileContent, "Photos", file.FileName);

                    // Sort order jako powtarzalne pola
                    var sortOrder = (model.PhotoSortOrders != null && model.PhotoSortOrders.Count > i)
                        ? model.PhotoSortOrders[i]
                        : i;
                    content.Add(new StringContent(sortOrder.ToString()), "PhotoSortOrders");
                }
            }

            if (model.Parameters != null)
            {
                foreach (var kvp in model.Parameters)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        content.Add(new StringContent(kvp.Value), $"Parameters[{kvp.Key}]");
                    }
                }
            }

            if (model.SelectedFeatureIds != null)
            {
                foreach (var id in model.SelectedFeatureIds)
                {
                    content.Add(new StringContent(id.ToString()), "SelectedFeatureIds");
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
        #endregion

        #region Update
        public async Task UpdateListing(CreateListingViewModel model)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Id.ToString()), "Id");
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
            content.Add(new StringContent(model.MainPhotoIndex.ToString()), "MainPhotoIndex");

            // Features
            if (model.SelectedFeatureIds != null)
            {
                foreach (var fid in model.SelectedFeatureIds)
                {
                    content.Add(new StringContent(fid.ToString()), "SelectedFeatureIds");
                }
            }

            // Parametry
            if (model.Parameters != null)
            {
                foreach (var kvp in model.Parameters)
                {
                    content.Add(new StringContent(kvp.Value ?? string.Empty), $"Parameters[{kvp.Key}]");
                }
            }

            if (model.Photos != null)
            {
                var photoList = model.Photos.ToList();
                for (int i = 0; i < photoList.Count; i++)
                {
                    var file = photoList[i];
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "Photos", file.FileName);

                    var sortOrder = (model.PhotoSortOrders != null && model.PhotoSortOrders.Count > i)
                        ? model.PhotoSortOrders[i]
                        : i;
                    content.Add(new StringContent(sortOrder.ToString()), "PhotoSortOrders");
                }
            }

            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/Listings/{model.Id}", content);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region StatusUpdate
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
        #endregion
    }
}
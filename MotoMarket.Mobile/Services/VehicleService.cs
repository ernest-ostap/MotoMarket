using System.Net.Http.Json;
using System.Net.Http.Headers;
using MotoMarket.Mobile.Models.Listings; 

namespace MotoMarket.Mobile.Services
{
    public class VehicleService
    {
        private readonly HttpClient _httpClient;

        public VehicleService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Constants.ApiUrl);
        }

        #region GetListing
        public async Task<IEnumerable<ListingDto>> GetAllListingsAsync(ListingsFilterDto filter = null)
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                _httpClient.DefaultRequestHeaders.Authorization = null;

                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var url = "api/Listings";

                if (filter != null)
                {
                    var queryParams = new List<string>();

                    if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
                        queryParams.Add($"searchCallback={Uri.EscapeDataString(filter.SearchQuery)}");

                    if (filter.PriceMin.HasValue)
                        queryParams.Add($"priceMin={filter.PriceMin}");

                    if (filter.PriceMax.HasValue)
                        queryParams.Add($"priceMax={filter.PriceMax}");

                    if (filter.BrandId.HasValue)
                        queryParams.Add($"brandId={filter.BrandId}");

                    if (filter.YearMin.HasValue)
                        queryParams.Add($"yearMin={filter.YearMin}");

                    if (filter.YearMax.HasValue)
                        queryParams.Add($"yearMax={filter.YearMax}");

                    if (queryParams.Any())
                    {
                        url += "?" + string.Join("&", queryParams);
                    }
                }

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ListingDto>>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API ERROR] {ex.Message}");
            }

            return new List<ListingDto>();
        }

        public async Task<IEnumerable<ListingDto>> GetMyListingsAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return new List<ListingDto>();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("api/Listings/mine");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ListingDto>>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MY LISTINGS ERROR] {ex.Message}");
            }
            return new List<ListingDto>();
        }

        public async Task<ListingDetailDto> GetListingDetailAsync(int id)
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                _httpClient.DefaultRequestHeaders.Authorization = null;

                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync($"api/Listings/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ListingDetailDto>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DETAIL ERROR] {ex.Message}");
            }
            return null;
        }
        #endregion

        #region Create
        public async Task<IEnumerable<DictionaryDto>> GetDictionaryAsync(string dictionaryName)
        {
            try
            {
                // Np. api/FuelTypes, api/GearboxTypes - dostosuj do swojego API!
                var response = await _httpClient.GetAsync($"api/{dictionaryName}");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<DictionaryDto>>();
            }
            catch { }
            return new List<DictionaryDto>();
        }

        // 2. Metoda CreateListingAsync obsługująca wszystkie pola
        public async Task<bool> CreateListingAsync(CreateListingDto dto, IEnumerable<FileResult> photos)
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return false;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var content = new MultipartFormDataContent();

                // Helper do dodawania stringów
                void AddString(string val, string name) => content.Add(new StringContent(val ?? ""), name);

                // --- Proste pola ---
                AddString(dto.Title, nameof(dto.Title));
                AddString(dto.Description, nameof(dto.Description));
                AddString(dto.VIN, nameof(dto.VIN));
                AddString(dto.LocationCity, nameof(dto.LocationCity));
                AddString(dto.LocationRegion, nameof(dto.LocationRegion));

                AddString(dto.Price.ToString(System.Globalization.CultureInfo.InvariantCulture), nameof(dto.Price));
                AddString(dto.ProductionYear.ToString(), nameof(dto.ProductionYear));
                AddString(dto.Mileage.ToString(), nameof(dto.Mileage));

                // --- Dropdowny ID ---
                AddString(dto.BrandId.ToString(), nameof(dto.BrandId));
                AddString(dto.ModelId.ToString(), nameof(dto.ModelId));
                AddString(dto.FuelTypeId.ToString(), nameof(dto.FuelTypeId));
                AddString(dto.GearboxTypeId.ToString(), nameof(dto.GearboxTypeId));
                AddString(dto.DriveTypeId.ToString(), nameof(dto.DriveTypeId));
                AddString(dto.BodyTypeId.ToString(), nameof(dto.BodyTypeId));
                AddString(dto.VehicleCategoryId.ToString(), nameof(dto.VehicleCategoryId));

                // --- Lista FeatureIds (Wyposażenie) ---
                if (dto.SelectedFeatureIds != null)
                {
                    foreach (var id in dto.SelectedFeatureIds)
                    {
                        content.Add(new StringContent(id.ToString()), "SelectedFeatureIds");
                    }
                }

                if (dto.Parameters != null)
                {
                    foreach (var kvp in dto.Parameters)
                    {
                        content.Add(new StringContent(kvp.Value), $"Parameters[{kvp.Key}]");
                    }
                }

                // --- Zdjęcia ---
                if (photos != null)
                {
                    foreach (var photo in photos)
                    {
                        var stream = await photo.OpenReadAsync();
                        var streamContent = new StreamContent(stream);
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(photo.ContentType);
                        content.Add(streamContent, "Photos", photo.FileName);
                    }
                }

                var response = await _httpClient.PostAsync("api/Listings", content);

                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"CREATE ERROR: {err}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CREATE EX: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Status
        public async Task<bool> ChangeListingStatusAsync(int id, ListingStatus newStatus)
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response;

                if (newStatus == ListingStatus.Active)
                {
                    response = await _httpClient.PostAsync($"api/Listings/{id}/restore", null);
                }
                else
                {
                    var command = new
                    {
                        Id = id,
                        Status = (int)newStatus
                    };

                    response = await _httpClient.PostAsJsonAsync($"api/Listings/{id}/status", command);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[STATUS API ERROR] {response.StatusCode}: {error}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[STATUS EXCEPTION] {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Favorites
        public async Task<bool?> ToggleFavoriteAsync(int listingId)
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return null; // Nie zalogowany

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // POST api/Favorites/{id}
                var response = await _httpClient.PostAsync($"api/Favorites/{listingId}", null);

                if (response.IsSuccessStatusCode)
                {
                    // API zwraca true (dodano) lub false (usunięto)
                    var result = await response.Content.ReadAsStringAsync();
                    return bool.Parse(result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FAV ERROR] {ex.Message}");
            }
            return null; // Błąd
        }

        public async Task<IEnumerable<ListingDto>> GetMyFavoritesAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return new List<ListingDto>();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("api/Favorites");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ListingDto>>();
                }
            }
            catch { }
            return new List<ListingDto>();
        }
        #endregion

        #region Helpers
        public async Task<IEnumerable<DictionaryDto>> GetBrandsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Brands");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<DictionaryDto>>();
                }
            }
            catch { }
            return new List<DictionaryDto>();
        }

        public async Task<IEnumerable<DictionaryDto>> GetModelsAsync(int brandId)
        {
            try
            {
                // Swagger mówi: GET /api/Models (bez parametrów)
                var response = await _httpClient.GetAsync("api/Models");

                if (response.IsSuccessStatusCode)
                {
                    var allModels = await response.Content.ReadFromJsonAsync<IEnumerable<DictionaryDto>>();

                    // FILTROWANIE PO STRONIE TELEFONU:
                    // Zwracamy tylko te, gdzie model.BrandId == wybrane brandId
                    return allModels.Where(x => x.BrandId == brandId).ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MODELS ERROR] {ex.Message}");
            }

            return new List<DictionaryDto>();
        }

        public async Task<IEnumerable<FeatureDto>> GetFeaturesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Dictionaries/features");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<FeatureDto>>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FEATURES ERROR] {ex.Message}");
            }
            return new List<FeatureDto>();
        }

        public async Task<IEnumerable<ParameterTypeDto>> GetParametersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Dictionaries/parameters");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ParameterTypeDto>>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PARAMETERS ERROR] {ex.Message}");
            }
            return new List<ParameterTypeDto>();
        }
        #endregion
    }
}
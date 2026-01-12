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
    }
}
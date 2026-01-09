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

        public async Task<IEnumerable<ListingDto>> GetAllListingsAsync()
        {
            try
            {
                // 1. POBIERZ TOKEN
                var token = await SecureStorage.GetAsync("auth_token");

                // Czyścimy nagłówek autoryzacji przed każdym zapytaniem
                _httpClient.DefaultRequestHeaders.Authorization = null;

                // 2. DOKLEJ GO DO NAGŁÓWKA (Bearer)
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                // 3. STRZAŁ DO API
                // Używamy endpointu, który zwraca listę (sprawdź czy to api/Listings czy api/Vehicles)
                var response = await _httpClient.GetAsync("api/Listings");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ListingDto>>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API ERROR] {ex.Message}");
            }

            return new List<ListingDto>(); // Zwróć pustą listę w razie błędu
        }
    }
}
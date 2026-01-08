using System.Net.Http.Json;
using MotoMarket.Mobile.Models.Auth;

namespace MotoMarket.Mobile.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Constants.ApiUrl);
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var command = new LoginUserCommand { Email = email, Password = password };

                // Logujemy co wysyłamy
                System.Diagnostics.Debug.WriteLine($"[AUTH] Próba logowania pod: {_httpClient.BaseAddress}/api/Users/login");

                var response = await _httpClient.PostAsJsonAsync("api/Users/login", command);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthDto>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        await SecureStorage.SetAsync("auth_token", result.Token);
                        return true;
                    }
                }
                else
                {
                    // Zobaczysz ten błąd w okienku "Output" w Visual Studio
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[AUTH BŁĄD API] Status: {response.StatusCode}, Treść: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                // To jest najważniejsze - pokaże dlaczego Android odrzucił połączenie
                System.Diagnostics.Debug.WriteLine($"[AUTH WYJĄTEK] {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[AUTH WYJĄTEK] StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[AUTH WYJĄTEK] InnerException: {ex.InnerException.Message}");
                }
            }
            return false;
        }

        public void Logout()
        {
            SecureStorage.Remove("auth_token");
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token);
        }
    }
}
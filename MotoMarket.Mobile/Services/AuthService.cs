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

        #region Login and logout user
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

        #endregion

        #region Register user
        public async Task<bool> RegisterAsync(string email, string password, string confirmPassword, string firstName, string lastName, string phone)
        {
            try
            {
                var command = new RegisterUserCommand
                {
                    Email = email,
                    Password = password,
                    ConfirmPassword = confirmPassword,
                    // Mapujemy nowe pola
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phone
                };

                var response = await _httpClient.PostAsJsonAsync("api/Users/register", command);

                if (response.IsSuccessStatusCode)
                {
                    // Opcjonalnie: Możesz tu od razu zalogować usera (pobrać token), 
                    // jeśli API zwraca token przy rejestracji. 
                    // Jeśli nie, zwracamy true i user musi się zalogować.
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[REGISTER ERROR] {error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[REGISTER EXCEPTION] {ex.Message}");
            }
            return false;
        }
        #endregion

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token);
        }
    }
}
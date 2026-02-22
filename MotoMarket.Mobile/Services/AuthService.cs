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

                //logs to debug api url
                //System.Diagnostics.Debug.WriteLine($"[AUTH] Próba logowania pod: {_httpClient.BaseAddress}/api/Users/login");

                var response = await _httpClient.PostAsJsonAsync("api/Users/login", command);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthDto>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        await SecureStorage.SetAsync("auth_token", result.Token);
                        await SecureStorage.SetAsync("user_id", result.Id);
                        await SecureStorage.SetAsync("user_first_name", result.FirstName ?? string.Empty);
                        await SecureStorage.SetAsync("user_last_name", result.LastName ?? string.Empty);
                        return true;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[AUTH BŁĄD API] Status: {response.StatusCode}, Treść: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine($"[AUTH WYJĄTEK] {ex.GetType().Name}: {ex.Message}");
                //System.Diagnostics.Debug.WriteLine($"[AUTH WYJĄTEK] StackTrace: {ex.StackTrace}");
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
            SecureStorage.Remove("user_id");
            SecureStorage.Remove("user_first_name");
            SecureStorage.Remove("user_last_name");
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
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phone
                };

                var response = await _httpClient.PostAsJsonAsync("api/Users/register", command);

                if (response.IsSuccessStatusCode)
                {
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

        // Zwraca imię i nazwisko użytkownika zapisane przy logowaniu (lub pusty string).
        public static async Task<string> GetUserDisplayNameAsync()
        {
            try
            {
                var firstName = await SecureStorage.GetAsync("user_first_name") ?? string.Empty;
                var lastName = await SecureStorage.GetAsync("user_last_name") ?? string.Empty;
                var full = $"{firstName} {lastName}".Trim();
                return string.IsNullOrEmpty(full) ? "Użytkownik" : full;
            }
            catch
            {
                return "Użytkownik";
            }
        }
    }
}
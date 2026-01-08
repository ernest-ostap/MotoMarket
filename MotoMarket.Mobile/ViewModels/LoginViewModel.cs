using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Services;

namespace MotoMarket.Mobile.ViewModels
{
    // [ObservableObject] to magia z Toolkitu. Generuje kod powiadamiania widoku.
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        public LoginViewModel()
        {
            _authService = new AuthService(); // Na inżynierkę robimy new, bez Dependency Injection dla uproszczenia
        }

        // [ObservableProperty] automatycznie tworzy właściwość 'Email' (z dużej litery),
        // do której podepniemy się w widoku.
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isBusy; // Żeby pokazać kółko ładowania

        // [RelayCommand] zamienia tę metodę w Komendę, którą może kliknąć przycisk
        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                // ZMIANA TUTAJ: Używamy Application.Current.MainPage zamiast Shell.Current
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wpisz email i hasło", "OK");
                return;
            }

            IsBusy = true;

            // Upewnij się, że _authService nie jest nullem (powinien być w konstruktorze)
            var success = await _authService.LoginAsync(Email, Password);

            IsBusy = false;

            if (success)
            {
                // ZMIANA TUTAJ:
                await Application.Current.MainPage.DisplayAlert("Sukces", "Zalogowano pomyślnie!", "Super");
            }
            else
            {
                // ZMIANA TUTAJ:
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nieprawidłowe dane logowania", "OK");
            }
        }
    }
}
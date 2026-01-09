using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Services;

namespace MotoMarket.Mobile.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        public RegisterViewModel()
        {
            _authService = new AuthService();
        }

        [ObservableProperty] string email;
        [ObservableProperty] string password;
        [ObservableProperty] string confirmPassword;
        [ObservableProperty] bool isBusy;

        [RelayCommand]
        async Task RegisterAsync()
        {
            if (IsBusy) return;

            // Prosta walidacja
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wypełnij wszystkie pola", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Hasła muszą być identyczne", "OK");
                return;
            }

            IsBusy = true;
            var success = await _authService.RegisterAsync(Email, Password, ConfirmPassword);
            IsBusy = false;

            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Sukces", "Konto utworzone! Zaloguj się.", "OK");
                // Wracamy do ekranu logowania (cofamy się w nawigacji)
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Rejestracja nieudana. Sprawdź dane.", "OK");
            }
        }
    }
}
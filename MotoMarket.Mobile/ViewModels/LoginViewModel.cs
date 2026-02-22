using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Services;

namespace MotoMarket.Mobile.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        public LoginViewModel()
        {
            _authService = new AuthService(); 
        }

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wpisz email i hasło", "OK");
                return;
            }

            IsBusy = true;

            var success = await _authService.LoginAsync(Email, Password);

            IsBusy = false;

            if (success)
            {
                Application.Current.MainPage = new NavigationPage(new Views.ListingsPage());
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nieprawidłowe dane logowania", "OK");
            }
        }

        [RelayCommand]
        async Task GoToRegisterAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Views.RegisterPage());
        }
    }
}
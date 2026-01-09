using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Services;
using MotoMarket.Mobile.Views;

namespace MotoMarket.Mobile.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        public ProfileViewModel()
        {
            _authService = new AuthService();
        }

        [RelayCommand]
        async Task GoToMyListingsAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new MyListingsPage());
        }

        [RelayCommand]
        void Logout()
        {
            _authService.Logout(); // Usuwa token

            // Przekierowanie na stronę główną i wyczyszczenie stosu nawigacji
            // Dzięki temu user nie może kliknąć "Wstecz" i wrócić do profilu
            Application.Current.MainPage = new NavigationPage(new ListingsPage());
        }
    }
}
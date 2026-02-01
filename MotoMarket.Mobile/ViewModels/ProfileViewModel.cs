using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Services;
using MotoMarket.Mobile.Views;

namespace MotoMarket.Mobile.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        string userDisplayName = "Zalogowany użytkownik";

        public ProfileViewModel()
        {
            _authService = new AuthService();
        }

        /// <summary>
        /// Wywołaj przy wejściu na stronę profilu, żeby odświeżyć imię i nazwisko.
        /// </summary>
        public async Task LoadUserDisplayNameAsync()
        {
            var name = await AuthService.GetUserDisplayNameAsync();
            UserDisplayName = string.IsNullOrWhiteSpace(name) || name == "Użytkownik"
                ? "Zalogowany użytkownik"
                : $"Zalogowano jako: {name}";
        }

        [RelayCommand]
        async Task GoToMyListingsAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new MyListingsPage());
        }

        [RelayCommand]
        async Task GoToChatsAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new MyChatsPage());
        }

        [RelayCommand]
        async Task GoToFavoritesAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Views.FavoritesPage());
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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Models.Listings;
using MotoMarket.Mobile.Services;
using System.Collections.ObjectModel;

namespace MotoMarket.Mobile.ViewModels
{
    public partial class ListingsViewModel : ObservableObject
    {
        private readonly VehicleService _vehicleService;

        public ListingsViewModel()
        {
            _vehicleService = new VehicleService();
            // Automatycznie załaduj dane przy starcie
            LoadListingsCommand.Execute(null);
        }

        // Kolekcja widoczna dla listy na ekranie
        public ObservableCollection<ListingDto> Listings { get; } = new();

        [ObservableProperty]
        bool isBusy;

        [RelayCommand]
        async Task LoadListingsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var items = await _vehicleService.GetAllListingsAsync();

                Listings.Clear();
                foreach (var item in items)
                {
                    // Tutaj mały hack dla zdjęć na localhost/emulatorze
                    // Jeśli URL zaczyna się od /, musimy dodać domenę API
                    if (!string.IsNullOrEmpty(item.MainPhotoUrl) && item.MainPhotoUrl.StartsWith("/"))
                    {
                        item.MainPhotoUrl = Constants.ApiUrl + item.MainPhotoUrl;
                    }

                    Listings.Add(item);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task GoToLoginAsync()
        {
            // Idziemy do ekranu logowania
            await Application.Current.MainPage.Navigation.PushAsync(new Views.LoginPage());
        }

        [RelayCommand]
        async Task CheckProfileAsync()
        {
            var authService = new AuthService(); // Szybka instancja
            var isLogged = await authService.IsAuthenticatedAsync();

            if (isLogged)
            {
                // Jak zalogowany -> Idź do Profilu
                await Application.Current.MainPage.Navigation.PushAsync(new Views.ProfilePage());
            }
            else
            {
                // Jak niezalogowany -> Idź do Logowania
                await Application.Current.MainPage.Navigation.PushAsync(new Views.LoginPage());
            }
        }

        [RelayCommand]
        async Task GoToDetailsAsync(ListingDto item)
        {
            if (item == null) return;

            // Przechodzimy do strony szczegółów, przekazując ID
            await Application.Current.MainPage.Navigation.PushAsync(new Views.ListingDetailPage(item.Id));
        }
    }
}
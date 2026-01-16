using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Models.Listings; // ListingDto
using MotoMarket.Mobile.Services;
using System.Collections.ObjectModel;

namespace MotoMarket.Mobile.ViewModels
{
    public partial class FavoritesViewModel : ObservableObject
    {
        private readonly VehicleService _vehicleService;

        public FavoritesViewModel()
        {
            _vehicleService = new VehicleService();
        }

        public ObservableCollection<ListingDto> Favorites { get; } = new();

        [ObservableProperty] bool isBusy;

        [RelayCommand]
        async Task LoadFavoritesAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            var items = await _vehicleService.GetMyFavoritesAsync();

            Favorites.Clear();
            foreach (var item in items)
            {
                // Fix URLi jeśli trzeba
                if (!string.IsNullOrEmpty(item.MainPhotoUrl) && item.MainPhotoUrl.StartsWith("/"))
                    item.MainPhotoUrl = Constants.ApiUrl + item.MainPhotoUrl;

                // Skoro pobraliśmy z ulubionych, to na pewno jest IsFavorite=true
                item.IsFavorite = true;
                Favorites.Add(item);
            }

            IsBusy = false;
        }

        // Nawigacja do szczegółów
        [RelayCommand]
        async Task GoToDetailsAsync(ListingDto item)
        {
            if (item == null) return;
            await Application.Current.MainPage.Navigation.PushAsync(new Views.ListingDetailPage(item.Id));
        }

        // Usuwanie z ulubionych (bezpośrednio z tej listy)
        [RelayCommand]
        async Task RemoveFavoriteAsync(ListingDto item)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Usuń", "Usunąć z ulubionych?", "Tak", "Nie");
            if (!confirm) return;

            var result = await _vehicleService.ToggleFavoriteAsync(item.Id);
            // Jeśli toggle zadziałał (zwrócił false = usunięto, lub cokolwiek), usuwamy z listy lokalnej
            Favorites.Remove(item);
        }
    }
}
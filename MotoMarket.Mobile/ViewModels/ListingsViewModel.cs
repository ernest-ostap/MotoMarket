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

        [ObservableProperty] string searchText;
        [ObservableProperty] string minPrice;
        [ObservableProperty] string maxPrice;
        [ObservableProperty] string yearMin;
        [ObservableProperty] string yearMax;

        [ObservableProperty] DictionaryDto selectedBrand;

        [ObservableProperty] bool areFiltersVisible;
        [ObservableProperty] bool isBusy;

        public ObservableCollection<ListingDto> Listings { get; } = new();
        public ObservableCollection<DictionaryDto> Brands { get; } = new();

        public ListingsViewModel()
        {
            _vehicleService = new VehicleService();

            Task.Run(async () =>
            {
                await LoadBrandsAsync();
                await SearchAsync();
            });
        }

        async Task LoadBrandsAsync()
        {
            var brands = await _vehicleService.GetBrandsAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Brands.Clear();
                foreach (var b in brands) Brands.Add(b);
            });
        }

        [RelayCommand]
        void ToggleFilters()
        {
            AreFiltersVisible = !AreFiltersVisible;
        }

        [RelayCommand]
        async Task SearchAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                decimal? pMin = decimal.TryParse(MinPrice, out var d1) ? d1 : null;
                decimal? pMax = decimal.TryParse(MaxPrice, out var d2) ? d2 : null;
                int? yMin = int.TryParse(YearMin, out var i1) ? i1 : null;
                int? yMax = int.TryParse(YearMax, out var i2) ? i2 : null;

                var filter = new ListingsFilterDto
                {
                    SearchQuery = SearchText,
                    PriceMin = pMin,
                    PriceMax = pMax,
                    YearMin = yMin,
                    YearMax = yMax,
                    BrandId = SelectedBrand?.Id
                };

                var items = await _vehicleService.GetAllListingsAsync(filter);

                Listings.Clear();
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.MainPhotoUrl) && item.MainPhotoUrl.StartsWith("/"))
                        item.MainPhotoUrl = Constants.ApiUrl + item.MainPhotoUrl;

                    Listings.Add(item);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task ClearFiltersAsync()
        {
            SearchText = string.Empty;
            MinPrice = string.Empty;
            MaxPrice = string.Empty;
            YearMin = string.Empty;
            YearMax = string.Empty;

            SelectedBrand = null;

            await SearchAsync();
        }


        [RelayCommand]
        async Task LoadListingsAsync() => await SearchAsync();

        [RelayCommand]
        async Task GoToLoginAsync() =>
            await Application.Current.MainPage.Navigation.PushAsync(new Views.LoginPage());

        [RelayCommand]
        async Task CheckProfileAsync()
        {
            var authService = new AuthService();
            if (await authService.IsAuthenticatedAsync())
                await Application.Current.MainPage.Navigation.PushAsync(new Views.ProfilePage());
            else
                await Application.Current.MainPage.Navigation.PushAsync(new Views.LoginPage());
        }

        [RelayCommand]
        async Task GoToDetailsAsync(ListingDto item)
        {
            if (item == null) return;
            await Application.Current.MainPage.Navigation.PushAsync(new Views.ListingDetailPage(item.Id));
        }

        [RelayCommand]
        async Task ToggleFavoriteAsync(ListingDto item)
        {
            if (item == null) return;

            // 1. Sprawdź czy zalogowany
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
            {
                await Application.Current.MainPage.DisplayAlert("Info", "Zaloguj się, aby dodawać do ulubionych.", "OK");
                await Application.Current.MainPage.Navigation.PushAsync(new Views.LoginPage());
                return;
            }

            // 2. Optymistyczna aktualizacja UI (zmień od razu, żeby user czuł reakcję)
            bool oldState = item.IsFavorite;
            item.IsFavorite = !item.IsFavorite;

            // 3. Strzał do API w tle
            var result = await _vehicleService.ToggleFavoriteAsync(item.Id);

            if (result.HasValue)
            {
                // Upewniamy się, że stan jest zgodny z serwerem
                item.IsFavorite = result.Value;
            }
            else
            {
                // Błąd? Cofnij zmianę
                item.IsFavorite = oldState;
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie udało się zaktualizować ulubionych.", "OK");
            }
        }
    }
}
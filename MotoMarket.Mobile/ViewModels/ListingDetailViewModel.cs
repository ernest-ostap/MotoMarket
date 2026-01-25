using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Models.Listings;
using MotoMarket.Mobile.Services;
using System.Collections.ObjectModel;

namespace MotoMarket.Mobile.ViewModels
{
    public partial class ListingDetailViewModel : ObservableObject
    {
        private readonly VehicleService _vehicleService;
        private int _listingId;

        public ListingDetailViewModel(int listingId)
        {
            _listingId = listingId;
            _vehicleService = new VehicleService();
            LoadDataCommand.Execute(null);
        }

        [ObservableProperty] ListingDetailDto listing;
        [ObservableProperty] bool isBusy;
        [ObservableProperty] bool hasParameters;
        [ObservableProperty] bool hasFeatures;

        // Kolekcja zdjęć do karuzeli
        public ObservableCollection<string> Photos { get; } = new();

        [RelayCommand]
        async Task LoadDataAsync()
        {
            IsBusy = true;
            var data = await _vehicleService.GetListingDetailAsync(_listingId);

            if (data != null)
            {
                // Fix URLi zdjęć (dla emulatora/tunelu)
                var fixedPhotos = new List<string>();
                foreach (var url in data.PhotoUrls)
                {
                    if (url.StartsWith("/"))
                        fixedPhotos.Add(Constants.ApiUrl + url);
                    else
                        fixedPhotos.Add(url);
                }
                data.PhotoUrls = fixedPhotos;

                // Wrzucamy do ObservableCollection dla karuzeli
                Photos.Clear();
                foreach (var p in fixedPhotos) Photos.Add(p);

                Listing = data;

                // Aktualizuj flagi widoczności sekcji
                HasParameters = data.Parameters != null && data.Parameters.Any();
                HasFeatures = data.Features != null && data.Features.Any();
            }
            IsBusy = false;
        }

        [RelayCommand]
        void CallSeller()
        {
            if (Listing == null || string.IsNullOrWhiteSpace(Listing.SellerPhone))
            {
                Application.Current.MainPage.DisplayAlert("Błąd", "Brak numeru telefonu", "OK");
                return;
            }

            try
            {
                // ZMIANA: Próbujemy otworzyć dialer "na siłę", nawet jak IsSupported mówi false.
                // Na prawdziwym telefonie to zadziała, a na emulatorze otworzy aplikację "Telefon".
                PhoneDialer.Default.Open(Listing.SellerPhone);
            }
            catch (Exception ex)
            {
                // Dopiero jak system rzuci błędem, to wyświetlamy alert
                Application.Current.MainPage.DisplayAlert("Błąd", "Nie udało się otworzyć telefonu: " + ex.Message, "OK");
            }
        }

        [RelayCommand]
        async Task GoBackAsync()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        [RelayCommand]
        async Task OpenChatAsync()
        {
            if (Listing == null || string.IsNullOrEmpty(Listing.UserId)) return;

            await Application.Current.MainPage.Navigation.PushAsync(
                new Views.ChatPage(Listing.UserId, Listing.Id));
        }

        [RelayCommand]
        async Task ToggleFavoriteAsync()
        {
            if (Listing == null) return;

            // Sprawdź logowanie
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
            {
                await Application.Current.MainPage.DisplayAlert("Info", "Zaloguj się, aby dodać do ulubionych.", "OK");
                return;
            }

            // Optymistyczna zmiana UI
            bool oldState = Listing.IsFavorite;
            Listing.IsFavorite = !Listing.IsFavorite;

            var result = await _vehicleService.ToggleFavoriteAsync(Listing.Id);

            if (result.HasValue)
            {
                Listing.IsFavorite = result.Value;
            }
            else
            {
                // Cofnij w razie błędu
                Listing.IsFavorite = oldState;
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie udało się zmienić statusu ulubionych", "OK");
            }
        }
    }
}
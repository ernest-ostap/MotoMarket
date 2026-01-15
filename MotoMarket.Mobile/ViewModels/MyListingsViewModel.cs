using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Models.Listings;
using MotoMarket.Mobile.Services;
using System.Collections.ObjectModel;

namespace MotoMarket.Mobile.ViewModels
{
    public partial class MyListingsViewModel : ObservableObject
    {
        private readonly VehicleService _vehicleService;

        public MyListingsViewModel()
        {
            _vehicleService = new VehicleService();
            LoadDataCommand.Execute(null);
        }

        public ObservableCollection<ListingDto> Listings { get; } = new();

        [ObservableProperty] bool isBusy;

        [RelayCommand]
        async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var items = await _vehicleService.GetMyListingsAsync();
                Listings.Clear();
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.MainPhotoUrl) && item.MainPhotoUrl.StartsWith("/"))
                        item.MainPhotoUrl = Constants.ApiUrl + item.MainPhotoUrl;

                    Listings.Add(item);
                }
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        async Task GoToCreateAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Views.CreateListingPage());
        }

        [RelayCommand]
        async Task ChangeStatusAsync(ListingDto item)
        {
            if (item == null) return;

            string action = "";
            ListingStatus newStatus = ListingStatus.Active;

            // Logika wyboru akcji
            if (item.Status == (int)ListingStatus.Active)
            {
                // Jeśli aktywne -> pytamy co zrobić
                action = await Application.Current.MainPage.DisplayActionSheet(
                    $"Zarządzaj: {item.Title}", "Anuluj", null,
                    "✅ Oznacz jako Sprzedane",
                    "📦 Archiwizuj (Ukryj)"); 
            }
            else
            {
                // Jeśli nieaktywne -> pytamy czy przywrócić
                action = await Application.Current.MainPage.DisplayActionSheet(
                    $"Zarządzaj: {item.Title}", "Anuluj", null,
                    "🔄 Przywróć do Aktywnych");
            }

            if (action == "Anuluj" || action == null) return;

            // Mapowanie wyboru na status
            if (action == "✅ Oznacz jako Sprzedane") newStatus = ListingStatus.Sold;
            else if (action == "📦 Archiwizuj (Ukryj)") newStatus = ListingStatus.Archived;
            else if (action == "🔄 Przywróć do Aktywnych") newStatus = ListingStatus.Active;

            // Strzał do API
            IsBusy = true;
            var success = await _vehicleService.ChangeListingStatusAsync(item.Id, newStatus);
            IsBusy = false;

            if (success)
            {
                item.Status = (int)newStatus;
                await LoadDataAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie udało się zmienić statusu.", "OK");
            }
        }
    }
}
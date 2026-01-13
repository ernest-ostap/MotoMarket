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
    }
}
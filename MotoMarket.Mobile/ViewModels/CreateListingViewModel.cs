using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Models.Listings;
using MotoMarket.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Mobile.ViewModels
{
    public partial class CreateListingViewModel : ObservableObject
    {
        private readonly VehicleService _vehicleService;

        public CreateListingViewModel()
        {
            _vehicleService = new VehicleService();
            Task.Run(LoadDictionariesAsync);
        }

        // --- POLA FORMULARZA ---
        [ObservableProperty] string title;
        [ObservableProperty] string description;
        [ObservableProperty] string price;
        [ObservableProperty] string vin;
        [ObservableProperty] string productionYear;
        [ObservableProperty] string mileage;
        [ObservableProperty] string city;
        [ObservableProperty] string region;

        // --- DROPDOWNY (Wybrane obiekty) ---
        [ObservableProperty] DictionaryDto selectedBrand;
        [ObservableProperty] DictionaryDto selectedModel;
        [ObservableProperty] DictionaryDto selectedFuel;
        [ObservableProperty] DictionaryDto selectedGearbox;
        [ObservableProperty] DictionaryDto selectedDrive;
        [ObservableProperty] DictionaryDto selectedBody;
        [ObservableProperty] DictionaryDto selectedCategory;

        // --- ŹRÓDŁA DANYCH (Listy do Pickerów) ---
        public ObservableCollection<DictionaryDto> Brands { get; } = new();
        public ObservableCollection<DictionaryDto> Models { get; } = new();
        public ObservableCollection<DictionaryDto> FuelTypes { get; } = new();
        public ObservableCollection<DictionaryDto> GearboxTypes { get; } = new();
        public ObservableCollection<DictionaryDto> DriveTypes { get; } = new();
        public ObservableCollection<DictionaryDto> BodyTypes { get; } = new();
        public ObservableCollection<DictionaryDto> Categories { get; } = new();

        // --- ZDJĘCIA ---
        public ObservableCollection<ImageSource> PhotosPreview { get; } = new();
        private List<FileResult> _rawPhotos = new();

        [ObservableProperty] bool isBusy;

        async Task LoadDictionariesAsync()
        {
            IsBusy = true;
            // UWAGA: Sprawdź nazwy endpointów w swoim API Controllerze (np. DictionariesController)
            var t1 = _vehicleService.GetBrandsAsync();
            var t2 = _vehicleService.GetDictionaryAsync("FuelTypes");
            var t3 = _vehicleService.GetDictionaryAsync("GearboxTypes");
            var t4 = _vehicleService.GetDictionaryAsync("DriveTypes");
            var t5 = _vehicleService.GetDictionaryAsync("BodyTypes");
            var t6 = _vehicleService.GetDictionaryAsync("VehicleCategories");

            await Task.WhenAll(t1, t2, t3, t4, t5, t6);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Brands.Clear(); foreach (var i in t1.Result) Brands.Add(i);

                FuelTypes.Clear(); foreach (var i in t2.Result) FuelTypes.Add(i);
                GearboxTypes.Clear(); foreach (var i in t3.Result) GearboxTypes.Add(i);
                DriveTypes.Clear(); foreach (var i in t4.Result) DriveTypes.Add(i);
                BodyTypes.Clear(); foreach (var i in t5.Result) BodyTypes.Add(i);
                Categories.Clear(); foreach (var i in t6.Result) Categories.Add(i);
            });
            IsBusy = false;
        }

        // Obsługa zmiany marki -> ładowanie modeli
        partial void OnSelectedBrandChanged(DictionaryDto value)
        {
            Models.Clear();
            SelectedModel = null;
            if (value != null)
            {
                Task.Run(async () =>
                {
                    var models = await _vehicleService.GetModelsAsync(value.Id);
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        foreach (var m in models) Models.Add(m);
                    });
                });
            }
        }

        [RelayCommand]
        async Task PickPhotosAsync()
        {
            var result = await FilePicker.PickMultipleAsync(new PickOptions { FileTypes = FilePickerFileType.Images });
            if (result != null)
            {
                foreach (var file in result)
                {
                    _rawPhotos.Add(file);
                    PhotosPreview.Add(ImageSource.FromFile(file.FullPath));
                }
            }
        }

        [RelayCommand]
        async Task SubmitAsync()
        {
            if (IsBusy) return;

            // 1. ROZSZERZONA WALIDACJA
            // Sprawdzamy czy WSZYSTKIE słowniki są wybrane
            if (SelectedBrand == null ||
                SelectedModel == null ||
                SelectedFuel == null ||
                SelectedGearbox == null ||
                SelectedBody == null ||
                SelectedDrive == null ||     
                SelectedCategory == null ||   
                string.IsNullOrWhiteSpace(Title) ||
                string.IsNullOrWhiteSpace(Price))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Uzupełnij wszystkie wymagane pola (w tym Napęd i Kategorię)!", "OK");
                return;
            }

            IsBusy = true;

            var dto = new CreateListingDto
            {
                Title = Title,
                Description = Description,
                Price = decimal.TryParse(Price, out var p) ? p : 0,
                ProductionYear = int.TryParse(ProductionYear, out var y) ? y : DateTime.Now.Year,
                Mileage = int.TryParse(Mileage, out var m) ? m : 0,
                VIN = Vin,
                LocationCity = City,
                LocationRegion = Region,

                BrandId = SelectedBrand.Id,
                ModelId = SelectedModel.Id,
                FuelTypeId = SelectedFuel.Id,
                GearboxTypeId = SelectedGearbox.Id,
                DriveTypeId = SelectedDrive.Id,
                BodyTypeId = SelectedBody.Id,
                VehicleCategoryId = SelectedCategory.Id,

                // TODO: FeatureIds i Parameters możesz dodać jeśli zrobisz dla nich UI w XAML
            };

            var success = await _vehicleService.CreateListingAsync(dto, _rawPhotos);

            IsBusy = false;

            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Sukces", "Ogłoszenie dodane!", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nie udało się dodać ogłoszenia", "OK");
            }
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Mobile.Models.Listings
{
    //przekopiowane z application
    public partial class ListingDto : ObservableObject
    {
        // --- Podstawowe ---
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; // Pełny opis
        public decimal Price { get; set; }
        public string VIN { get; set; } = string.Empty;

        //  Dane techniczne 
        public int ProductionYear { get; set; }
        public int Mileage { get; set; }

        //  Lokalizacja 
        public string LocationCity { get; set; } = string.Empty;
        public string LocationRegion { get; set; } = string.Empty;

        //  Status i Daty 
        public int Status { get; set; } // zwracamy int, a front end to obsluzy z enum
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        // --- Słowniki (Spłaszczone nazwy zamiast ID) ---
        public string FuelTypeName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string VehicleCategoryName { get; set; } = string.Empty;
        public string GearboxTypeName { get; set; } = string.Empty;
        public string DriveTypeName { get; set; } = string.Empty;
        public string BodyTypeName { get; set; } = string.Empty;

        // --- Zdjęcia ---
        public string MainPhotoUrl { get; set; } = string.Empty;

        [ObservableProperty]
        bool isFavorite;
    }
}

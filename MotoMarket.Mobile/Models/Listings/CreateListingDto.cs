using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Mobile.Models.Listings
{
    public class CreateListingDto
    {
        // --- Podstawowe ---
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string VIN { get; set; }

        // --- Dane techniczne (Inty/Liczby) ---
        public int ProductionYear { get; set; }
        public int Mileage { get; set; }

        // --- Lokalizacja ---
        public string LocationCity { get; set; }
        public string LocationRegion { get; set; }

        // --- Klucze obce ---
        public int BrandId { get; set; }
        public int ModelId { get; set; }

        public int FuelTypeId { get; set; }
        public int GearboxTypeId { get; set; }
        public int DriveTypeId { get; set; }
        public int BodyTypeId { get; set; }
        public int VehicleCategoryId { get; set; }

        public List<int> SelectedFeatureIds { get; set; } = new();

        public Dictionary<int, string> Parameters { get; set; } = new();
    }
}

using MotoMarket.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveType = MotoMarket.Domain.Entities.Vehicles.DriveType;

namespace MotoMarket.Domain.Entities.Listings
{
    public class Listing : Common.BaseEntity
    {
        public string UserId { get; set; } = string.Empty; //z identity, trzymany jako string
        
        //Dane ogloszenia, szczegoly, lokalizacja, status
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string VIN { get; set; } = string.Empty;
        public int ProductionYear { get; set; }
        public int Mileage { get; set; }
        public string LocationCity { get; set; } = string.Empty;
        public string LocationRegion { get; set; } = string.Empty;
        public ListingStatus Status { get; set; } //zdefiniowany ponizej

        //Dane pojazdu z FK
        public int BrandId { get; set; }
        public VehicleBrand Brand { get; set; } = null!;

        public int ModelId { get; set; }
        public VehicleModel Model { get; set; } = null!;

        public int VehicleCategoryId { get; set; }
        public VehicleCategory VehicleCategory { get; set; } = null!;

        public int FuelTypeId { get; set; }
        public FuelType FuelType { get; set; } = null!;

        public int GearboxTypeId { get; set; }
        public GearboxType GearboxType { get; set; } = null!;

        public int DriveTypeId { get; set; }
        public DriveType DriveType { get; set; } = null!;

        public int BodyTypeId { get; set; }
        public BodyType BodyType { get; set; } = null!;

        //daty
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        //kolekcje
        
        public ICollection<ListingPhoto> Photos { get; set; } = new List<ListingPhoto>();
        public ICollection<ListingFeature> Features { get; set; } = new List<ListingFeature>();
    }

    public enum ListingStatus
    {
        Draft = 0,
        Active = 1,
        Sold = 2,
        Archived = 3,
        Banned = 4
    }
}

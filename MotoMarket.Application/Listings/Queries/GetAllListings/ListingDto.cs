using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Queries.GetAllListings
{
    public class ListingDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string VIN { get; set; } = string.Empty;

        public int ProductionYear { get; set; }
        public int Mileage { get; set; }

        public string LocationCity { get; set; } = string.Empty;
        public string LocationRegion { get; set; } = string.Empty;

        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public string FuelTypeName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string VehicleCategoryName { get; set; } = string.Empty;
        public string GearboxTypeName { get; set; } = string.Empty;
        public string DriveTypeName { get; set; } = string.Empty;
        public string BodyTypeName { get; set; } = string.Empty;

        public string MainPhotoUrl { get; set; } = string.Empty;

        public bool IsFavorite { get; set; }
    }
}

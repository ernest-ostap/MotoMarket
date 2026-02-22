using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MotoMarket.Domain.Entities;

namespace MotoMarket.Application.Listings.Commands.CreateListing
{
    //Returns created listing id.
    public record CreateListingCommand : IRequest<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public int BrandId { get; set; }
        public int ModelId { get; set; }
        public int VehicleCategoryId { get; set; }
        public int FuelTypeId { get; set; }
        public int GearboxTypeId { get; set; }
        public int DriveTypeId { get; set; }
        public int BodyTypeId { get; set; }

        public string VIN { get; set; } = string.Empty;
        public int ProductionYear { get; set; }
        public int Mileage { get; set; }
        public string LocationCity { get; set; } = string.Empty;
        public string LocationRegion { get; set; } = string.Empty;

        public List<ListingPhotoInput> Photos { get; set; } = new();
        public List<string> PhotoUrls { get; set; } = new List<string>();
        public Dictionary<int, string> Parameters { get; set; } = new Dictionary<int, string>();
        public List<int> SelectedFeatureIds { get; set; } = new List<int>();
    }

    public class ListingPhotoInput
    {
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
    }
}

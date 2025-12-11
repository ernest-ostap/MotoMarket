using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MotoMarket.Application.Listings.Commands.CreateListing;
using MotoMarket.Domain.Entities;

namespace MotoMarket.Application.Listings.Commands.UpdateListing
{
    // Zwracamy Unit (czyli nic, void), bo przy update HTTP 204 No Content jest standardem
    public class UpdateListingCommand : IRequest
    {
        public int Id { get; set; } //id jest kluczowe, bo musimy wiedziec ktorego robic update

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        // Możliwość zmiany parametrów technicznych
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

        // Zdjęcia (opcjonalnie przy update)
        public List<ListingPhotoInput> Photos { get; set; } = new();
        public List<string> PhotoUrls { get; set; } = new(); // fallback legacy
    }
}

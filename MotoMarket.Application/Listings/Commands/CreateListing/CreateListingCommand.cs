using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MotoMarket.Domain.Entities; // Potrzebne do enumów (np. FuelType)

namespace MotoMarket.Application.Listings.Commands.CreateListing
{
    // Komenda zwraca int - id utworzonego rekordu
    public record CreateListingCommand : IRequest<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        // ID słowników (użytkownik wybierze je z listy)
        public int BrandId { get; set; }
        public int ModelId { get; set; }
        public int VehicleCategoryId { get; set; }
        public int FuelTypeId { get; set; }
        public int GearboxTypeId { get; set; }
        public int DriveTypeId { get; set; }
        public int BodyTypeId { get; set; }

        // Dane techniczne
        public string VIN { get; set; } = string.Empty;
        public int ProductionYear { get; set; }
        public int Mileage { get; set; }
        public string LocationCity { get; set; } = string.Empty;
        public string LocationRegion { get; set; } = string.Empty;

        public List<string> PhotoUrls { get; set; } = new List<string>();
    }
}

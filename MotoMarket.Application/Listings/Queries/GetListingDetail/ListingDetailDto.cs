using MotoMarket.Application.Listings.Queries.GetAllListings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Queries.GetListingDetail
{
    public class ListingDetailDto : ListingDto
    {
        public int BrandId { get; set; }
        public int ModelId { get; set; }
        public int FuelTypeId { get; set; }
        public int GearboxTypeId { get; set; }
        public int BodyTypeId { get; set; }
        public int DriveTypeId { get; set; }
        public int VehicleCategoryId { get; set; }

        public IEnumerable<string> PhotoUrls { get; set; } = new List<string>();
        public IEnumerable<string> Features { get; set; } = new List<string>();

        public IEnumerable<int> FeatureIds { get; set; } = new List<int>();
        public IEnumerable<ListingParameterDto> Parameters { get; set; } = new List<ListingParameterDto>();

        public string SellerPhone { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
    }

    public class ListingParameterDto
    {
        public int ParameterTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }
}

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
        // Dodatkowe pola, których nie ma na liście
        public IEnumerable<string> PhotoUrls { get; set; } = new List<string>();
        public IEnumerable<string> Features { get; set; } = new List<string>(); 
        public IEnumerable<ListingParameterDto> Parameters { get; set; } = new List<ListingParameterDto>();
    }

    public class ListingParameterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;  // np. "kWh" (Warto to dodać!)
    }
}

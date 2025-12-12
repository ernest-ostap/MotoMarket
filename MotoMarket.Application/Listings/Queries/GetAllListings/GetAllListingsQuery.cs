using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MotoMarket.Application.Listings.Queries.GetAllListings
{
    // Zwracamy liste (IEnumerable) z obiektow DTO
    public class GetAllListingsQuery : IRequest<IEnumerable<ListingDto>>
    {
        public string? SearchCallback { get; set; } // Szukanie w tytule
        public int? BrandId { get; set; }
        public int? ModelId { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public int? YearMin { get; set; }
        public int? YearMax { get; set; }

        //sortowanie (np. "price_asc", "newest")
        public string? SortBy { get; set; }
    }
}

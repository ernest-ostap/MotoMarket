using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MotoMarket.Application.Listings.Queries.GetAllListings
{
    public class GetAllListingsQuery : IRequest<IEnumerable<ListingDto>>
    {
        public string? SearchCallback { get; set; }
        public int? BrandId { get; set; }
        public int? ModelId { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public int? YearMin { get; set; }
        public int? YearMax { get; set; }

        /// <summary>Sort key (e.g. price_asc, newest).</summary>
        public string? SortBy { get; set; }
    }
}

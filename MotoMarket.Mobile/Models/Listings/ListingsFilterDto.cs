using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Mobile.Models.Listings
{
    public class ListingsFilterDto
    {
        public string SearchQuery { get; set; } = string.Empty;
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public int? YearMin { get; set; }
        public int? BrandId { get; set; }
        public int? ModelId { get; set; }
    }
}

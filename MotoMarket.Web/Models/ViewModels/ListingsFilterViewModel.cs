using Microsoft.AspNetCore.Mvc.Rendering;
using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Web.Models.ViewModels
{
    public class ListingsFilterViewModel
    {
        public string? Search { get; set; } // Szukanie w tytule
        public int? BrandId { get; set; }
        public int? ModelId { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public int? YearMin { get; set; }
        public int? YearMax { get; set; }
        public string? SortBy { get; set; } = "newest";

        public IEnumerable<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Models { get; set; } = new List<SelectListItem>();

        public IEnumerable<ListingDto> Listings { get; set; } = new List<ListingDto>();
    }
}

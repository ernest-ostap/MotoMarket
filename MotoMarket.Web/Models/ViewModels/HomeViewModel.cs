using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Web.Models.ViewModels
{
    public class HomeViewModel
    {
        public string WelcomeTitle { get; set; } = "Witaj w MotoMarket";
        public string WelcomeSubtitle { get; set; } = "Znajdź swój wymarzony samochód";

        public IEnumerable<DictionaryDto> Brands { get; set; } = new List<DictionaryDto>();

        public IEnumerable<ListingDto> RecentListings { get; set; } = new List<ListingDto>();
    }
}
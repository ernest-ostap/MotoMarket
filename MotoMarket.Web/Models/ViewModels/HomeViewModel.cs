using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Web.Models.ViewModels
{
    public class HomeViewModel
    {
        // Treści z CMS
        public string WelcomeTitle { get; set; } = "Witaj w MotoMarket";
        public string WelcomeSubtitle { get; set; } = "Znajdź swój wymarzony samochód";

        // Dane do wyszukiwarki (Dropdown marek)
        public IEnumerable<DictionaryDto> Brands { get; set; } = new List<DictionaryDto>();

        // Kafelki z autami
        public IEnumerable<ListingDto> RecentListings { get; set; } = new List<ListingDto>();
    }
}
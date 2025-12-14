using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Models.ViewModels;

namespace MotoMarket.Web.Services.Listings
{
    public interface IVehicleService
    {
        Task<IEnumerable<ListingDto>> GetAllListings(ListingsFilterViewModel filter);
        Task<ListingDetailDto?> GetListingDetail(int id);
        Task CreateListing(CreateListingViewModel model);
        Task<IEnumerable<ListingDto>> GetMyListings();
        Task UpdateListing(CreateListingViewModel model);
        Task DeleteListing(int id);
        Task RestoreListing(int id);
        Task UpdateListingStatus(int id, int status);
        Task<bool> ToggleFavorite(int listingId); // Zwraca true=dodano, false=usunięto
        Task<IEnumerable<ListingDto>> GetMyFavorites();
    }
}

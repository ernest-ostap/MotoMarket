using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Models.ViewModels;

namespace MotoMarket.Web.Services.Listings
{
    public interface IVehicleService
    {
        Task<IEnumerable<ListingDto>> GetAllListings();
        Task<ListingDetailDto?> GetListingDetail(int id);
        Task CreateListing(CreateListingViewModel model);

    }
}

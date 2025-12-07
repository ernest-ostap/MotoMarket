using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Web.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<ListingDto>> GetAllListings();
        Task<ListingDetailDto?> GetListingDetail(int id);

    }
}

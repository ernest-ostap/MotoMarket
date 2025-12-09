using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Web.Services.Listings
{
    public interface IDictionaryService
    {
        Task<IEnumerable<SelectListItemDto>> GetBrands();
        Task<IEnumerable<SelectListItemDto>> GetModels(int brandId);
        Task<IEnumerable<SelectListItemDto>> GetBodyTypes();
        Task<IEnumerable<SelectListItemDto>> GetDriveTypes();
        Task<IEnumerable<SelectListItemDto>> GetFuelTypes();
        Task<IEnumerable<SelectListItemDto>> GetGearboxTypes();
        Task<IEnumerable<SelectListItemDto>> GetVehicleCategories();

        
    }
}

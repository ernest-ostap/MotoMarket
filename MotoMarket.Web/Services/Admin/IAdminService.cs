using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Web.Services.Admin
{
    public interface IAdminService
    {
        #region brands
        Task<IEnumerable<DictionaryDto>> GetAllBrands();
        Task<DictionaryDto?> GetBrand(int id);
        Task<bool> CreateBrand(string name);
        Task<bool> UpdateBrand(int id, string name);
        Task<bool> ToggleBrandActive(int id);
        Task<bool> DeleteBrand(int id);
        #endregion

        #region models

        #endregion

        #region asd

        #endregion

        #region MyRegion

        #endregion

        #region MyRegion

        #endregion

        #region MyRegion

        #endregion

        #region MyRegion

        #endregion

        #region MyRegion

        #endregion

        #region MyRegion

        #endregion

        #region MyRegion

        #endregion

    }
}
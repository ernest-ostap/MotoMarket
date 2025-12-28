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
        Task<IEnumerable<ModelDto>> GetAllModels();
        Task<ModelDto?> GetModel(int id);
        Task<bool> CreateModel(string name, int brandId);
        Task<bool> UpdateModel(int id, string name, int brandId);
        Task<bool> ToggleModelActive(int id);
        Task<bool> DeleteModel(int id);
        #endregion

        #region DriveTypes
        Task<IEnumerable<DictionaryDto>> GetAllDriveTypes();
        Task<DictionaryDto?> GetDriveType(int id);
        Task<bool> CreateDriveType(string name);
        Task<bool> UpdateDriveType(int id, string name);
        Task<bool> ToggleDriveTypeActive(int id);
        Task<bool> DeleteDriveType(int id);
        #endregion

        #region BodyTypes
        Task<IEnumerable<DictionaryDto>> GetAllBodyTypes();
        Task<DictionaryDto?> GetBodyType(int id);
        Task<bool> CreateBodyType(string name);
        Task<bool> UpdateBodyType(int id, string name);
        Task<bool> ToggleBodyTypeActive(int id);
        Task<bool> DeleteBodyType(int id);

        #endregion

        #region FuelTypes
        Task<IEnumerable<DictionaryDto>> GetAllFuelTypes();
        Task<DictionaryDto?> GetFuelType(int id);
        Task<bool> CreateFuelType(string name);
        Task<bool> UpdateFuelType(int id, string name);
        Task<bool> ToggleFuelTypeActive(int id);
        Task<bool> DeleteFuelType(int id);
        #endregion

        #region GearboxTypes
        Task<IEnumerable<DictionaryDto>> GetAllGearboxTypes();
        Task<DictionaryDto?> GetGearboxType(int id);
        Task<bool> CreateGearboxType(string name);
        Task<bool> UpdateGearboxType(int id, string name);
        Task<bool> ToggleGearboxTypeActive(int id);
        Task<bool> DeleteGearboxType(int id);
        #endregion

        #region VehicleCategories
        Task<IEnumerable<DictionaryDto>> GetAllVehicleCategories();
        Task<DictionaryDto?> GetVehicleCategory(int id);
        Task<bool> CreateVehicleCategory(string name);
        Task<bool> UpdateVehicleCategory(int id, string name);
        Task<bool> ToggleVehicleCategoryActive(int id);
        Task<bool> DeleteVehicleCategory(int id);
        #endregion

        #region Features
        Task <IEnumerable<FeatureDto>> GetAllFeatures();
        Task<FeatureDto?> GetFeature(int id);
        Task<bool> CreateFeature(string name, string icon);
        Task<bool> UpdateFeature(int id, string name, string icon);
        Task<bool> ToggleFeatureActive(int id);
        Task<bool> DeleteFeature(int id);

        #endregion

        #region ParametersTypes

        #endregion

    }
}
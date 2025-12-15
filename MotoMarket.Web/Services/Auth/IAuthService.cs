using MotoMarket.Web.Models.ViewModels;

namespace MotoMarket.Web.Services.Auth
{
    public interface IAuthService
    {
        Task<bool> Login(LoginViewModel model);
        Task<bool> Register(RegisterViewModel model);
        Task Logout();
        Task<bool> UpdateProfile(UpdateProfileViewModel model);
        Task<bool> ChangePassword(ChangePasswordViewModel model);
    }
}
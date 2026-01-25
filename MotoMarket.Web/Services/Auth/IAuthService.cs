using MotoMarket.Web.Models.ViewModels;

namespace MotoMarket.Web.Services.Auth
{
    public interface IAuthService
    {
        Task<string?> Login(LoginViewModel model);
        Task<bool> Register(RegisterViewModel model);
        Task Logout();
        Task<UpdateProfileViewModel> GetUserProfile();
        Task<bool> UpdateProfile(UpdateProfileViewModel model);
        Task<bool> ChangePassword(ChangePasswordViewModel model);

        Task<bool> IsUserBanned(string userId);
    }
}
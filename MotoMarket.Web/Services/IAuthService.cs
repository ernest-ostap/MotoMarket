using MotoMarket.Web.Models.ViewModels;

namespace MotoMarket.Web.Services
{
    public interface IAuthService
    {
        Task<bool> Login(LoginViewModel model);
        Task<bool> Register(RegisterViewModel model);
        Task Logout();
    }
}
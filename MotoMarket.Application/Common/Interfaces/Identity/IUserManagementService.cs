using MotoMarket.Application.Users.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Common.Interfaces.Identity
{
    public interface IUserManagementService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> ToggleBanAsync(string userId);
        Task<bool> IsUserBanned(string userId);
        Task<bool> ToggleAdminRoleAsync(string userId);
    }
}
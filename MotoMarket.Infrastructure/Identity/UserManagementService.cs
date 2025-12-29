using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Users.Queries;
using MotoMarket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Infrastructure.Identity
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                // Pobieramy rolę (zakładamy, że user ma jedną główną rolę)
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "User";

                // Sprawdzamy czy zbanowany (LockoutEnd > Teraz)
                var isBanned = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;

                result.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = role,
                    IsBanned = isBanned,
                    CreatedAt =  user.RegisteredAt
                });
            }

            return result;
        }

        public async Task<bool> ToggleBanAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // Sprawdzamy czy już zbanowany
            var isCurrentlyBanned = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;

            if (isCurrentlyBanned)
            {
                // ODBLOKUJ (Ustawiamy koniec blokady na "teraz")
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            else
            {
                // ZBANUJ (Ustawiamy koniec blokady na rok 9999)
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }

            return true;
        }

        public async Task<bool> IsUserBanned(string userId)
        {
            // FindByIdAsync korzysta z cache'a wewn. Identity, więc jest dość szybkie
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;
        }
    }
}

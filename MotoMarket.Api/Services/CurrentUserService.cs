using MotoMarket.Application.Common.Interfaces.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace MotoMarket.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string? UserId
        {
            get
            {
                // User ID from JWT (Sub or NameIdentifier)
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null) return null;

                return user.FindFirstValue(JwtRegisteredClaimNames.Sub) 
                    ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
            }
        }
    }
}

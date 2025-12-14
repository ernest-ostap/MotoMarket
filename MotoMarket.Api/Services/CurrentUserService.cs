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
                // Wyciąganie ID usera z tokena JWT
                // TokenService używa JwtRegisteredClaimNames.Sub, więc sprawdzamy oba
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null) return null;
                
                // Najpierw sprawdzamy Sub (jak w TokenService), potem NameIdentifier (na wypadek mapowania)
                return user.FindFirstValue(JwtRegisteredClaimNames.Sub) 
                    ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
            }
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks; // Potrzebne do Task

namespace MotoMarket.Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        // ZMIANA 1: Wstrzykujemy UserManager w konstruktorze
        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        // ZMIANA 2: Metoda musi być ASYNC, bo pobieranie ról to zapytanie do bazy
        public async Task<string> CreateToken(ApplicationUser user)
        {
            // 1. Dodawanie do tokena informacji o użytkowniku
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            // ZMIANA 3: Pobieramy role z bazy (to brakowało!)
            var userRoles = await _userManager.GetRolesAsync(user);

            // Dodaj role użytkownika jako osobne claims
            foreach (var role in userRoles)
            {
                // Używamy standardowego ClaimTypes.Role, żeby [Authorize(Roles="Admin")] działało z automatu
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 2. Pobieranie klucza z konfiguracji
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Konfiguracja tokena
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            // 4. Generowanie stringa
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
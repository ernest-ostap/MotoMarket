using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Users.Queries;
using MotoMarket.Domain.Entities;

namespace MotoMarket.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Sprawdź, czy email już istnieje
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new Exception("Użytkownik o tym adresie email już istnieje."); // TBD: BadRequestException
            }

            // 2. Utwórz obiekt użytkownika
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email, // Identity wymaga UserName (często to samo co email)
                FirstName = request.FirstName,
                LastName = request.LastName,
                RegisteredAt = DateTime.UtcNow
            };

            // 3. Zapisz w bazie (CreateAsync samo hashuje hasło!)
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                // Zbieramy błędy (np. hasło nie ma dużej litery)
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Błąd rejestracji: {errors}");
            }

            // 4. Wygeneruj token (żeby po rejestracji był od razu zalogowany)
            var token = _tokenService.CreateToken(user);

            return new AuthDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                Token = token
            };
        }
    }
}

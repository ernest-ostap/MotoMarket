using MediatR;
using Microsoft.AspNetCore.Identity;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Users.Queries;
using MotoMarket.Domain.Entities;

namespace MotoMarket.Application.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public LoginUserCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Znajdź usera po emailu
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                // Bezpieczeństwo: Nie mówimy "nie ma usera", tylko "błędne dane", żeby haker nie zgadywał maili.
                throw new UnauthorizedAccessException("Błędny email lub hasło.");
            }

            // 2. Sprawdź hasło
            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
            {
                throw new UnauthorizedAccessException("Błędny email lub hasło.");
            }

            // 3. Wygeneruj token
            var token = await _tokenService.CreateToken(user);

            return new AuthDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                Token = token
            };
        }
    }
}
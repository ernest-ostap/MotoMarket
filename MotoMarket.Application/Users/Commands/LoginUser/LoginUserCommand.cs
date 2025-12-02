using MediatR;
using MotoMarket.Application.Users.Queries;

namespace MotoMarket.Application.Users.Commands.LoginUser
{
    public class LoginUserCommand : IRequest<AuthDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
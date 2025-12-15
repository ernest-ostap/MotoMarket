using MediatR;

namespace MotoMarket.Application.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommand : IRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}


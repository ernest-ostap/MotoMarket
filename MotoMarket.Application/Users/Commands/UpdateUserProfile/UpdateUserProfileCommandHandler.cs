using MediatR;
using Microsoft.AspNetCore.Identity;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Domain.Entities;

namespace MotoMarket.Application.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserProfileCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new UnauthorizedAccessException();

            bool changed = false;

            if (!string.Equals(user.FirstName, request.FirstName, StringComparison.Ordinal))
            {
                user.FirstName = request.FirstName;
                changed = true;
            }
            if (!string.Equals(user.LastName, request.LastName, StringComparison.Ordinal))
            {
                user.LastName = request.LastName;
                changed = true;
            }
            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                // Ensure unique email
                var existing = await _userManager.FindByEmailAsync(request.Email);
                if (existing != null && existing.Id != user.Id)
                {
                    throw new InvalidOperationException("Email jest już zajęty.");
                }
                user.Email = request.Email;
                user.UserName = request.Email;
                changed = true;
            }
            if (!string.Equals(user.PhoneNumber ?? string.Empty, request.PhoneNumber ?? string.Empty, StringComparison.Ordinal))
            {
                user.PhoneNumber = request.PhoneNumber;
                changed = true;
            }

            if (changed)
            {
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException(errors);
                }
            }
        }
    }
}


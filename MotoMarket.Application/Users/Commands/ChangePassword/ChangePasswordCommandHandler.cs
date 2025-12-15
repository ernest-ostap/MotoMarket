using MediatR;
using Microsoft.AspNetCore.Identity;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Domain.Entities;

namespace MotoMarket.Application.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new UnauthorizedAccessException();

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }
        }
    }
}


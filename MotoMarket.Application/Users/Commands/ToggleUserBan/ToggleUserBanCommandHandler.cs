using MediatR;
using MotoMarket.Application.Common.Interfaces.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Users.Commands.ToggleUserBan
{
    public class ToggleUserBanCommandHandler : IRequestHandler<ToggleUserBanCommand>
    {
        private readonly IUserManagementService _userService;
        public ToggleUserBanCommandHandler(IUserManagementService userService) => _userService = userService;

        public async Task Handle(ToggleUserBanCommand request, CancellationToken token)
        {
            await _userService.ToggleBanAsync(request.UserId);
        }
    }
}

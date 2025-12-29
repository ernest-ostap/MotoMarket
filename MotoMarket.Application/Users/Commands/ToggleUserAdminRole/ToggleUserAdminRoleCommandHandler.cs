using MediatR;
using MotoMarket.Application.Common.Interfaces.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Users.Commands.ToggleUserAdminRole
{
    public class ToggleUserAdminRoleCommandHandler : IRequestHandler<ToggleUserAdminRoleCommand>
    {
        private readonly IUserManagementService _userService;
        public ToggleUserAdminRoleCommandHandler(IUserManagementService userService) => _userService = userService;

        public async Task Handle(ToggleUserAdminRoleCommand request, CancellationToken token)
        {
            await _userService.ToggleAdminRoleAsync(request.UserId);
        }
    }
}

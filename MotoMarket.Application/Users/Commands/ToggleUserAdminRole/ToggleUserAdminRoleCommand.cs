using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Users.Commands.ToggleUserAdminRole
{
    public record ToggleUserAdminRoleCommand(string UserId) : IRequest;
}

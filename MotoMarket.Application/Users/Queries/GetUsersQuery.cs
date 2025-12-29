using MediatR;
using MotoMarket.Application.Common.Interfaces.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Users.Queries
{
    public record GetUsersQuery : IRequest<IEnumerable<UserDto>>;

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserManagementService _userService;
        public GetUsersQueryHandler(IUserManagementService userService) => _userService = userService;

        public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken token)
        {
            return await _userService.GetAllUsersAsync();
        }
    }
}

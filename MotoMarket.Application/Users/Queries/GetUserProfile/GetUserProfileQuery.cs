using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MotoMarket.Application.Users.Queries.GetUserProfile
{
    public record GetUserProfileQuery : IRequest<UserProfileDto>;
}

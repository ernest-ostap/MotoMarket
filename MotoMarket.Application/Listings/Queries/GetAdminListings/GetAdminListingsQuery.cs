using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Queries.GetAdminListings
{
    public record GetAdminListingsQuery : IRequest<IEnumerable<AdminListingDto>>;
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MotoMarket.Application.Listings.Queries.GetAllListings
{
    // Zwracamy liste (IEnumerable) z obiektow DTO
    public record GetAllListingsQuery : IRequest<IEnumerable<ListingDto>>;
}

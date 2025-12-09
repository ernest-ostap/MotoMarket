using MediatR;
using MotoMarket.Application.Listings.Queries.GetAllListings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Queries.GetMyListings
{
    public record GetMyListingsQuery : IRequest<IEnumerable<ListingDto>>;
    
}
    

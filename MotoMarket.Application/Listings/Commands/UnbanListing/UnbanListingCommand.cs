using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Commands.UnbanListing
{
    public record UnbanListingCommand(int Id) : IRequest;
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Commands.BanListing
{
    public record BanListingCommand(int Id, string Reason) : IRequest;
}

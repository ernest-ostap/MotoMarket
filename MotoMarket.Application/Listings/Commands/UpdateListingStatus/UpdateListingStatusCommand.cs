using MediatR;
using MotoMarket.Domain.Entities.Listings;

namespace MotoMarket.Application.Listings.Commands.UpdateListingStatus
{
    public record UpdateListingStatusCommand(int Id, ListingStatus Status) : IRequest;
}


using MediatR;

namespace MotoMarket.Application.Listings.Commands.RestoreListing
{
    public record RestoreListingCommand(int Id) : IRequest;
}


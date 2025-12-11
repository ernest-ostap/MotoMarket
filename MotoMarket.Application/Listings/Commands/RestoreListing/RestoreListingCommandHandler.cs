using MediatR;
using MotoMarket.Application.Common.Exceptions;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Listings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Commands.RestoreListing
{
    public class RestoreListingCommandHandler : IRequestHandler<RestoreListingCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public RestoreListingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task Handle(RestoreListingCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Listings.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Listing), request.Id);
            }

            if (string.IsNullOrEmpty(_currentUserService.UserId) || entity.UserId != _currentUserService.UserId)
            {
                throw new UnauthorizedAccessException("Brak uprawnień do przywrócenia ogłoszenia.");
            }

            entity.Status = ListingStatus.Active;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}


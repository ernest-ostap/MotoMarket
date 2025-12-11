using MediatR;
using MotoMarket.Application.Common.Exceptions;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Listings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Commands.UpdateListingStatus
{
    public class UpdateListingStatusCommandHandler : IRequestHandler<UpdateListingStatusCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateListingStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdateListingStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Listings.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Listing), request.Id);
            }

            if (string.IsNullOrEmpty(_currentUserService.UserId) || entity.UserId != _currentUserService.UserId)
            {
                throw new UnauthorizedAccessException("Brak uprawnień do zmiany statusu ogłoszenia.");
            }

            // Ograniczamy możliwe statusy z poziomu UI (Active, Sold, Archived)
            if (request.Status != ListingStatus.Active &&
                request.Status != ListingStatus.Sold &&
                request.Status != ListingStatus.Archived)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Status), "Status nieobsługiwany.");
            }

            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}


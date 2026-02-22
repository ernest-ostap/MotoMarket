using MediatR;
using MotoMarket.Application.Common.Exceptions;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Domain.Entities.Listings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Commands.DeleteListing
{
    public class DeleteListingCommandHandler : IRequestHandler<DeleteListingCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DeleteListingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task Handle(DeleteListingCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Listings.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                
                throw new NotFoundException(nameof(Listing), request.Id);
            }

            if (string.IsNullOrEmpty(_currentUserService.UserId) || entity.UserId != _currentUserService.UserId)
            {
                throw new UnauthorizedAccessException("Brak uprawnień do usunięcia ogłoszenia.");
            }

            _context.Listings.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

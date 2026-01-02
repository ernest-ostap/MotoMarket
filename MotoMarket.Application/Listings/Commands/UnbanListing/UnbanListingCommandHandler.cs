using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Listings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Commands.UnbanListing
{
    public class UnbanListingCommandHandler : IRequestHandler<UnbanListingCommand>
    {
        private readonly IApplicationDbContext _context;

        public UnbanListingCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UnbanListingCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Listings.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                entity.Status = ListingStatus.Archived;
                entity.BanReason = null;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

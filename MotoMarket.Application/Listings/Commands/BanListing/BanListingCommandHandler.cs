using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Listings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Commands.BanListing
{
    public class BanListingCommandHandler : IRequestHandler<BanListingCommand>
    {
        private readonly IApplicationDbContext _context;

        public BanListingCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(BanListingCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Listings.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                entity.Status = ListingStatus.Banned;
                entity.BanReason = request.Reason;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

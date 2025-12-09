using MediatR;
using MotoMarket.Application.Common.Exceptions;
using MotoMarket.Application.Common.Interfaces.Persistence;
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

        public DeleteListingCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteListingCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Listings.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                
                throw new NotFoundException(nameof(Listing), request.Id);
            }

            // fizyczne usiniecie, zamiast zmiany statusu - nie będzie to uzywane w "codziennym uzyciu"
            //do zmiany statusu bede uzywal update command
            _context.Listings.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

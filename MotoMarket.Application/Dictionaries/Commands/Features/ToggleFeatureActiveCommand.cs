using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.Features
{
    public record ToggleFeatureActiveCommand(int Id) : IRequest;

    public class ToggleFeatureActiveCommandHandler : IRequestHandler<ToggleFeatureActiveCommand>
    {
        private readonly IApplicationDbContext _context;

        public ToggleFeatureActiveCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ToggleFeatureActiveCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.VehicleFeatures.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                entity.IsActive = !entity.IsActive;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

using MediatR;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Commands.Models
{
    public record ToggleModelActiveCommand(int Id) : IRequest;

    public class ToggleModelActiveCommandHandler : IRequestHandler<ToggleModelActiveCommand>
    {
        private readonly IApplicationDbContext _context;

        public ToggleModelActiveCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ToggleModelActiveCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.VehicleModels.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                entity.IsActive = !entity.IsActive;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}


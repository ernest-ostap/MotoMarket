using MediatR;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Commands.VehicleCategories
{
    public record ToggleVehicleCategoryActiveCommand(int Id) : IRequest;

    public class ToggleVehicleCategoryActiveCommandHandler : IRequestHandler<ToggleVehicleCategoryActiveCommand>
    {
        private readonly IApplicationDbContext _context;

        public ToggleVehicleCategoryActiveCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ToggleVehicleCategoryActiveCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.VehicleCategories.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                // Magia: Odwracamy wartość (jak było true to false, jak false to true)
                entity.IsActive = !entity.IsActive;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}


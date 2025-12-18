using MediatR;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Commands.FuelTypes
{
    public record ToggleFuelTypeActiveCommand(int Id) : IRequest;

    public class ToggleFuelTypeActiveCommandHandler : IRequestHandler<ToggleFuelTypeActiveCommand>
    {
        private readonly IApplicationDbContext _context;

        public ToggleFuelTypeActiveCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ToggleFuelTypeActiveCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.FuelTypes.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                // Magia: Odwracamy wartość (jak było true to false, jak false to true)
                entity.IsActive = !entity.IsActive;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}


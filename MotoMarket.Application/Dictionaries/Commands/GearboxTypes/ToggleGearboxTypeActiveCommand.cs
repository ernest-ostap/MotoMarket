using MediatR;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Commands.GearboxTypes
{
    public record ToggleGearboxTypeActiveCommand(int Id) : IRequest;

    public class ToggleGearboxTypeActiveCommandHandler : IRequestHandler<ToggleGearboxTypeActiveCommand>
    {
        private readonly IApplicationDbContext _context;

        public ToggleGearboxTypeActiveCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ToggleGearboxTypeActiveCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.GearboxTypes.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                entity.IsActive = !entity.IsActive;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}


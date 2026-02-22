using MediatR;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Commands.Brands
{
    public record ToggleBrandActiveCommand(int Id) : IRequest;

    public class ToggleBrandActiveCommandHandler : IRequestHandler<ToggleBrandActiveCommand>
    {
        private readonly IApplicationDbContext _context;

        public ToggleBrandActiveCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ToggleBrandActiveCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.VehicleBrands.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                entity.IsActive = !entity.IsActive;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
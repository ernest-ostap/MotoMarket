using MediatR;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Commands.Models
{
    public record UpdateModelCommand(int Id, string Name, int BrandId) : IRequest;

    public class UpdateModelCommandHandler : IRequestHandler<UpdateModelCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateModelCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateModelCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.VehicleModels.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                entity.Name = request.Name;
                entity.VehicleBrandId = request.BrandId; // Aktualizujemy też markę (gdyby ktoś się pomylił)
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
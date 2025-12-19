using MediatR;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Vehicles;

namespace MotoMarket.Application.Dictionaries.Commands.Models
{
    public record CreateModelCommand(string Name, int BrandId) : IRequest<int>;

    public class CreateModelCommandHandler : IRequestHandler<CreateModelCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateModelCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateModelCommand request, CancellationToken cancellationToken)
        {
            var entity = new VehicleModel
            {
                Name = request.Name,
                VehicleBrandId = request.BrandId, // Przypisujemy ID marki
                IsActive = true
            };

            _context.VehicleModels.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
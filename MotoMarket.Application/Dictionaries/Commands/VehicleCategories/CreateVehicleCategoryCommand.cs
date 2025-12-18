using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.VehicleCategories
{
    public record CreateVehicleCategoryCommand(string Name) : IRequest<int>;

    public class CreateVehicleCategoryCommandHandler : IRequestHandler<CreateVehicleCategoryCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateVehicleCategoryCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateVehicleCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = new VehicleCategory
            {
                Name = request.Name,
                IsActive = true // Domyślnie aktywna
            };

            _context.VehicleCategories.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}


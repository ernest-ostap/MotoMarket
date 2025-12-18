using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.VehicleCategories
{
    public record UpdateVehicleCategoryCommand(int Id, string Name) : IRequest;

    public class UpdateVehicleCategoryCommandHandler : IRequestHandler<UpdateVehicleCategoryCommand>
    {
        private readonly IApplicationDbContext _context;
        public UpdateVehicleCategoryCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(UpdateVehicleCategoryCommand request, CancellationToken token)
        {
            var entity = await _context.VehicleCategories.FindAsync(new object[] { request.Id }, token);
            if (entity == null) throw new Exception("Not found"); // Lub NotFoundException

            entity.Name = request.Name;
            await _context.SaveChangesAsync(token);
        }
    }
}


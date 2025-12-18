using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.VehicleCategories
{
    public record DeleteVehicleCategoryCommand(int Id) : IRequest;

    public class DeleteVehicleCategoryCommandHandler : IRequestHandler<DeleteVehicleCategoryCommand>
    {
        private readonly IApplicationDbContext _context;
        public DeleteVehicleCategoryCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(DeleteVehicleCategoryCommand request, CancellationToken token)
        {
            var entity = await _context.VehicleCategories.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                _context.VehicleCategories.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
        }
    }
}


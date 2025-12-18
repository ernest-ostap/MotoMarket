using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.FuelTypes
{
    public record CreateFuelTypeCommand(string Name) : IRequest<int>;

    public class CreateFuelTypeCommandHandler : IRequestHandler<CreateFuelTypeCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateFuelTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateFuelTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = new FuelType
            {
                Name = request.Name,
                IsActive = true // Domyślnie aktywna
            };

            _context.FuelTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}


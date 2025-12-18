using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.GearboxTypes
{
    public record CreateGearboxTypeCommand(string Name) : IRequest<int>;

    public class CreateGearboxTypeCommandHandler : IRequestHandler<CreateGearboxTypeCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateGearboxTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateGearboxTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = new GearboxType
            {
                Name = request.Name,
                IsActive = true // Domyślnie aktywna
            };

            _context.GearboxTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}


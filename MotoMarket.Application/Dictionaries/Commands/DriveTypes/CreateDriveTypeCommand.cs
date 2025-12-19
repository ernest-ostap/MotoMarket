using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveType = MotoMarket.Domain.Entities.Vehicles.DriveType;

namespace MotoMarket.Application.Dictionaries.Commands.DriveTypes
{
    public record CreateDriveTypeCommand(string Name) : IRequest<int>;

    public class CreateDriveTypeCommandHandler : IRequestHandler<CreateDriveTypeCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateDriveTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateDriveTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = new DriveType
            {
                Name = request.Name,
                IsActive = true // Domyślnie aktywna
            };

            _context.DriveTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}


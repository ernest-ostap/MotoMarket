using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.DriveTypes
{
    public record UpdateDriveTypeCommand(int Id, string Name) : IRequest;

    public class UpdateDriveTypeCommandHandler : IRequestHandler<UpdateDriveTypeCommand>
    {
        private readonly IApplicationDbContext _context;
        public UpdateDriveTypeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(UpdateDriveTypeCommand request, CancellationToken token)
        {
            var entity = await _context.DriveTypes.FindAsync(new object[] { request.Id }, token);
            if (entity == null) throw new Exception("Not found"); // Lub NotFoundException

            entity.Name = request.Name;
            await _context.SaveChangesAsync(token);
        }
    }
}


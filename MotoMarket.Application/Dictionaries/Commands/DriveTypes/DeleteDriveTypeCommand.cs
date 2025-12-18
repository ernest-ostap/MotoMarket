using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.DriveTypes
{
    public record DeleteDriveTypeCommand(int Id) : IRequest;

    public class DeleteDriveTypeCommandHandler : IRequestHandler<DeleteDriveTypeCommand>
    {
        private readonly IApplicationDbContext _context;
        public DeleteDriveTypeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(DeleteDriveTypeCommand request, CancellationToken token)
        {
            var entity = await _context.DriveTypes.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                _context.DriveTypes.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
        }
    }
}


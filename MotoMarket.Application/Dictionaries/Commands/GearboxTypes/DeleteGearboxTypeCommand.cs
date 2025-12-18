using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.GearboxTypes
{
    public record DeleteGearboxTypeCommand(int Id) : IRequest;

    public class DeleteGearboxTypeCommandHandler : IRequestHandler<DeleteGearboxTypeCommand>
    {
        private readonly IApplicationDbContext _context;
        public DeleteGearboxTypeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(DeleteGearboxTypeCommand request, CancellationToken token)
        {
            var entity = await _context.GearboxTypes.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                _context.GearboxTypes.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
        }
    }
}


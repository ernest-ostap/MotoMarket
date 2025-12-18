using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.GearboxTypes
{
    public record UpdateGearboxTypeCommand(int Id, string Name) : IRequest;

    public class UpdateGearboxTypeCommandHandler : IRequestHandler<UpdateGearboxTypeCommand>
    {
        private readonly IApplicationDbContext _context;
        public UpdateGearboxTypeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(UpdateGearboxTypeCommand request, CancellationToken token)
        {
            var entity = await _context.GearboxTypes.FindAsync(new object[] { request.Id }, token);
            if (entity == null) throw new Exception("Not found"); // Lub NotFoundException

            entity.Name = request.Name;
            await _context.SaveChangesAsync(token);
        }
    }
}


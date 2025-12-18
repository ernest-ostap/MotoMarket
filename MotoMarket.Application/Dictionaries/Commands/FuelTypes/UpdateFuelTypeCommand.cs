using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.FuelTypes
{
    public record UpdateFuelTypeCommand(int Id, string Name) : IRequest;

    public class UpdateFuelTypeCommandHandler : IRequestHandler<UpdateFuelTypeCommand>
    {
        private readonly IApplicationDbContext _context;
        public UpdateFuelTypeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(UpdateFuelTypeCommand request, CancellationToken token)
        {
            var entity = await _context.FuelTypes.FindAsync(new object[] { request.Id }, token);
            if (entity == null) throw new Exception("Not found"); // Lub NotFoundException

            entity.Name = request.Name;
            await _context.SaveChangesAsync(token);
        }
    }
}


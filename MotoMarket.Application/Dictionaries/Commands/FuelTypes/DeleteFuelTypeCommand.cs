using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.FuelTypes
{
    public record DeleteFuelTypeCommand(int Id) : IRequest;

    public class DeleteFuelTypeCommandHandler : IRequestHandler<DeleteFuelTypeCommand>
    {
        private readonly IApplicationDbContext _context;
        public DeleteFuelTypeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(DeleteFuelTypeCommand request, CancellationToken token)
        {
            var entity = await _context.FuelTypes.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                _context.FuelTypes.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
        }
    }
}


using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.ParameterTypes
{
    public record ToggleParameterTypeActiveCommand(int Id) : IRequest;

    public class ToggleParameterTypeActiveCommandHandler : IRequestHandler<ToggleParameterTypeActiveCommand>
    {
        private readonly IApplicationDbContext _context;

        public ToggleParameterTypeActiveCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ToggleParameterTypeActiveCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.VehicleParameterTypes.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                // Magia: Odwracamy wartość (jak było true to false, jak false to true)
                entity.IsActive = !entity.IsActive;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

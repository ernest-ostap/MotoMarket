using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.ParameterTypes
{
    public record DeleteParameterTypeCommand(int Id) : IRequest;

    public class DeleteParameterTypeCommandHandler : IRequestHandler<DeleteParameterTypeCommand>
    {
        private readonly IApplicationDbContext _context;
        public DeleteParameterTypeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(DeleteParameterTypeCommand request, CancellationToken token)
        {
            var entity = await _context.VehicleParameterTypes.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                _context.VehicleParameterTypes.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
        }
    }
}

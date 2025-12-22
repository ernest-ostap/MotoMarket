using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.Features
{
    public record DeleteFeatureCommand(int Id) : IRequest;

    public class DeleteFeatureCommandHandler : IRequestHandler<DeleteFeatureCommand>
    {
        private readonly IApplicationDbContext _context;
        public DeleteFeatureCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(DeleteFeatureCommand request, CancellationToken token)
        {
            var entity = await _context.VehicleFeatures.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                _context.VehicleFeatures.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
        }
    }
}

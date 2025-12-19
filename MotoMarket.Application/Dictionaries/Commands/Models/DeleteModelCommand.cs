using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.Models
{
    public record DeleteModelCommand(int Id) : IRequest;

    public class DeleteModelCommandHandler : IRequestHandler<DeleteModelCommand>
    {
        private readonly IApplicationDbContext _context;
        public DeleteModelCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(DeleteModelCommand request, CancellationToken token)
        {
            var entity = await _context.VehicleModels.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                _context.VehicleModels.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
        }
    }
}


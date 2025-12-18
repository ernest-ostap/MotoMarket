using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.Brands
{
    public record DeleteBrandCommand(int Id) : IRequest;

    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand>
    {
        private readonly IApplicationDbContext _context;
        public DeleteBrandCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(DeleteBrandCommand request, CancellationToken token)
        {
            var entity = await _context.VehicleBrands.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                _context.VehicleBrands.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
        }
    }
}

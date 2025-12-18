using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.Brands
{
    public record UpdateBrandCommand(int Id, string Name) : IRequest;

    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand>
    {
        private readonly IApplicationDbContext _context;
        public UpdateBrandCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(UpdateBrandCommand request, CancellationToken token)
        {
            var entity = await _context.VehicleBrands.FindAsync(new object[] { request.Id }, token);
            if (entity == null) throw new Exception("Not found"); // Lub NotFoundException

            entity.Name = request.Name;
            await _context.SaveChangesAsync(token);
        }
    }
}

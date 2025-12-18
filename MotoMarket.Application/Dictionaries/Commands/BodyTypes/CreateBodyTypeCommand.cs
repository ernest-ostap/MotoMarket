using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.BodyTypes
{
    public record CreateBodyTypeCommand(string Name) : IRequest<int>;

    public class CreateBodyTypeCommandHandler : IRequestHandler<CreateBodyTypeCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateBodyTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateBodyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = new BodyType
            {
                Name = request.Name,
                IsActive = true // Domyślnie aktywna
            };

            _context.BodyTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}


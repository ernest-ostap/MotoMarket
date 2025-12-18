using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.BodyTypes
{
    public record UpdateBodyTypeCommand(int Id, string Name) : IRequest;

    public class UpdateBodyTypeCommandHandler : IRequestHandler<UpdateBodyTypeCommand>
    {
        private readonly IApplicationDbContext _context;
        public UpdateBodyTypeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(UpdateBodyTypeCommand request, CancellationToken token)
        {
            var entity = await _context.BodyTypes.FindAsync(new object[] { request.Id }, token);
            if (entity == null) throw new Exception("Not found"); // Lub NotFoundException

            entity.Name = request.Name;
            await _context.SaveChangesAsync(token);
        }
    }
}


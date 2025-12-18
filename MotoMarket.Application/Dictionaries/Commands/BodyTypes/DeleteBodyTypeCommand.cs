using MediatR;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Commands.BodyTypes
{
    public record DeleteBodyTypeCommand(int Id) : IRequest;

    public class DeleteBodyTypeCommandHandler : IRequestHandler<DeleteBodyTypeCommand>
    {
        private readonly IApplicationDbContext _context;
        public DeleteBodyTypeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(DeleteBodyTypeCommand request, CancellationToken token)
        {
            var entity = await _context.BodyTypes.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                _context.BodyTypes.Remove(entity);
                await _context.SaveChangesAsync(token);
            }
        }
    }
}


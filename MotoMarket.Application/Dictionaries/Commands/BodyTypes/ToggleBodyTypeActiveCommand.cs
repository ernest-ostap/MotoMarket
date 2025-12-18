using MediatR;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Commands.BodyTypes
{
    public record ToggleBodyTypeActiveCommand(int Id) : IRequest;

    public class ToggleBodyTypeActiveCommandHandler : IRequestHandler<ToggleBodyTypeActiveCommand>
    {
        private readonly IApplicationDbContext _context;

        public ToggleBodyTypeActiveCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ToggleBodyTypeActiveCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.BodyTypes.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                // Magia: Odwracamy wartość (jak było true to false, jak false to true)
                entity.IsActive = !entity.IsActive;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}


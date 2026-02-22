using MediatR;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Commands.DriveTypes
{
    public record ToggleDriveTypeActiveCommand(int Id) : IRequest;

    public class ToggleDriveTypeActiveCommandHandler : IRequestHandler<ToggleDriveTypeActiveCommand>
    {
        private readonly IApplicationDbContext _context;

        public ToggleDriveTypeActiveCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ToggleDriveTypeActiveCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.DriveTypes.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity != null)
            {
                entity.IsActive = !entity.IsActive;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}


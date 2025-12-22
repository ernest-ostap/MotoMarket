using MediatR;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Commands.Features
{
    public record UpdateFeatureCommand(int Id, string Name, string GroupName) : IRequest;

    public class UpdateFeatureCommandHandler : IRequestHandler<UpdateFeatureCommand>
    {
        private readonly IApplicationDbContext _context;
        public UpdateFeatureCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task Handle(UpdateFeatureCommand request, CancellationToken token)
        {
            var entity = await _context.VehicleFeatures.FindAsync(new object[] { request.Id }, token);
            if (entity != null)
            {
                entity.Name = request.Name;
                entity.GroupName = request.GroupName;
                await _context.SaveChangesAsync(token);
            }
        }
    }
}
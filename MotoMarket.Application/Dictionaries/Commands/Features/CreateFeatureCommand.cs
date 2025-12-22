using MediatR;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Vehicles;

namespace MotoMarket.Application.Dictionaries.Commands.Features
{
    public record CreateFeatureCommand(string Name, string GroupName) : IRequest<int>;

    public class CreateFeatureCommandHandler : IRequestHandler<CreateFeatureCommand, int>
    {
        private readonly IApplicationDbContext _context;
        public CreateFeatureCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<int> Handle(CreateFeatureCommand request, CancellationToken token)
        {
            var entity = new VehicleFeature { Name = request.Name, GroupName = request.GroupName, IsActive = true };
            _context.VehicleFeatures.Add(entity);
            await _context.SaveChangesAsync(token);
            return entity.Id;
        }
    }
}
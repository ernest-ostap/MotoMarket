using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Application.Dictionaries.Queries
{
    public record GetAllFeaturesQuery(bool IncludeInactive = false) : IRequest<IEnumerable<FeatureDto>>;

    public class GetAllFeaturesQueryHandler : IRequestHandler<GetAllFeaturesQuery, IEnumerable<FeatureDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetAllFeaturesQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<FeatureDto>> Handle(GetAllFeaturesQuery request, CancellationToken token)
        {
            var query = _context.VehicleFeatures.AsNoTracking().AsQueryable();
            if (!request.IncludeInactive) query = query.Where(x => x.IsActive);

            return await query.OrderBy(x => x.GroupName).ThenBy(x => x.Name)
                .Select(x => new FeatureDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    GroupName = x.GroupName,
                    IsActive = x.IsActive
                })
                .ToListAsync(token);
        }
    }
}
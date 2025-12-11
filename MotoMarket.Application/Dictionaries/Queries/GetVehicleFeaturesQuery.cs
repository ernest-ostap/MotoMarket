using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Dictionaries.Queries
{
    public record GetVehicleFeaturesQuery : IRequest<IEnumerable<FeatureDto>>;
    public class GetVehicleFeaturesQueryHandler : IRequestHandler<GetVehicleFeaturesQuery, IEnumerable<FeatureDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetVehicleFeaturesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<FeatureDto>> Handle(GetVehicleFeaturesQuery request, CancellationToken cancellationToken)
        {
            return await _context.VehicleFeatures
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.SortOrder)
                .Select(x => new FeatureDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    GroupName = x.GroupName
                })
                .ToListAsync(cancellationToken);
        }
    }
}

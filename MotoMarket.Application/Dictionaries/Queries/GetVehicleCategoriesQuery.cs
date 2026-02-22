using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Queries
{
    /// IncludeInactive: false = only active (default), true = admin sees all.
    public record GetVehicleCategoriesQuery(bool IncludeInactive) : IRequest<IEnumerable<DictionaryDto>>;

    public class GetVehicleCategoriesQueryHandler : IRequestHandler<GetVehicleCategoriesQuery, IEnumerable<DictionaryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetVehicleCategoriesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DictionaryDto>> Handle(GetVehicleCategoriesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.VehicleCategories.AsNoTracking().AsQueryable();

            if (!request.IncludeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query
                .OrderBy(x => x.Name)
                .Select(x => new DictionaryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }
}


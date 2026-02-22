using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Queries
{
    /// <summary>IncludeInactive: false = only active (default), true = admin sees all.</summary>
    public record GetFuelTypesQuery(bool IncludeInactive) : IRequest<IEnumerable<DictionaryDto>>;

    public class GetFuelTypesQueryHandler : IRequestHandler<GetFuelTypesQuery, IEnumerable<DictionaryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetFuelTypesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DictionaryDto>> Handle(GetFuelTypesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.FuelTypes.AsNoTracking().AsQueryable();

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


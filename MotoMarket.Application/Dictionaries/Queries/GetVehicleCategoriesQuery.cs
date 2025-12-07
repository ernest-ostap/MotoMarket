using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces;

namespace MotoMarket.Application.Dictionaries.Queries
{
    public record GetVehicleCategoriesQuery : IRequest<IEnumerable<DictionaryDto>>;

    public class GetVehicleCategoriesQueryHandler : IRequestHandler<GetVehicleCategoriesQuery, IEnumerable<DictionaryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetVehicleCategoriesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DictionaryDto>> Handle(GetVehicleCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.VehicleCategories
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new DictionaryDto { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken);
        }
    }
}


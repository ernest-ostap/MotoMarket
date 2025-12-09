using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Queries
{
    public record GetModelsByBrandQuery(int BrandId) : IRequest<IEnumerable<DictionaryDto>>;

    public class GetModelsByBrandQueryHandler : IRequestHandler<GetModelsByBrandQuery, IEnumerable<DictionaryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetModelsByBrandQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DictionaryDto>> Handle(GetModelsByBrandQuery request, CancellationToken cancellationToken)
        {
            return await _context.VehicleModels
                .AsNoTracking()
                .Where(x => x.VehicleBrandId == request.BrandId && x.IsActive) // Filtrujemy po VehicleBrandId marki
                .OrderBy(x => x.Name)
                .Select(x => new DictionaryDto { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken);
        }
    }
}
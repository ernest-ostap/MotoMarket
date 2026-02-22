using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Application.Dictionaries.Queries
{
    /// <summary>IncludeInactive: false = only active (default), true = admin sees all.</summary>
    public record GetAllModelsQuery(bool IncludeInactive = false) : IRequest<IEnumerable<ModelDto>>;

    public class GetAllModelsQueryHandler : IRequestHandler<GetAllModelsQuery, IEnumerable<ModelDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllModelsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModelDto>> Handle(GetAllModelsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.VehicleModels
                .AsNoTracking()
                .Include(x => x.VehicleBrand) 
                .AsQueryable();

            if (!request.IncludeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query
                .OrderBy(x => x.VehicleBrand.Name) // Sortujemy po Marce
                .ThenBy(x => x.Name)               // Potem po Modelu
                .Select(x => new ModelDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    BrandId = x.VehicleBrandId,      // Używam nazwy z Twojego snippeta
                    BrandName = x.VehicleBrand.Name  // Mapujemy nazwę marki
                })
                .ToListAsync(cancellationToken);
        }
    }
}
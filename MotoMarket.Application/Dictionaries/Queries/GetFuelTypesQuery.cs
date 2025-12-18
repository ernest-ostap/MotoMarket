using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Queries
{
    // parametr IncludeInactive jest domyslne na FALSE, czyli zwykly user zawsze zobaczy tylko aktywne obiekty
    // ale za to wywołując to dla admina, dajemy to na true i admin widzi wszystko 
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
            // Budujemy zapytanie krok po kroku
            var query = _context.FuelTypes.AsNoTracking().AsQueryable();

            // 2. Jeśli NIE chcemy nieaktywnych (czyli jesteśmy zwykłym userem), to filtrujemy
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
                    // WAŻNE: Musisz zwracać IsActive, żeby Admin widział status!
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }
}


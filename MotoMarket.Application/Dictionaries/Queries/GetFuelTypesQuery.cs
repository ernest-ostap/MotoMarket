using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Queries
{
    public record GetFuelTypesQuery : IRequest<IEnumerable<DictionaryDto>>;

    public class GetFuelTypesQueryHandler : IRequestHandler<GetFuelTypesQuery, IEnumerable<DictionaryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetFuelTypesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DictionaryDto>> Handle(GetFuelTypesQuery request, CancellationToken cancellationToken)
        {
            return await _context.FuelTypes
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new DictionaryDto { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken);
        }
    }
}


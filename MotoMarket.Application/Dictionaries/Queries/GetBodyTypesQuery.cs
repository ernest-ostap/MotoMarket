using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces;

namespace MotoMarket.Application.Dictionaries.Queries
{
    public record GetBodyTypesQuery : IRequest<IEnumerable<DictionaryDto>>;

    public class GetBodyTypesQueryHandler : IRequestHandler<GetBodyTypesQuery, IEnumerable<DictionaryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetBodyTypesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DictionaryDto>> Handle(GetBodyTypesQuery request, CancellationToken cancellationToken)
        {
            return await _context.BodyTypes
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new DictionaryDto { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken);
        }
    }
}


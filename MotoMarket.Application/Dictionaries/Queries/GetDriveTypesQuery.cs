using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces;

namespace MotoMarket.Application.Dictionaries.Queries
{
    public record GetDriveTypesQuery : IRequest<IEnumerable<DictionaryDto>>;

    public class GetDriveTypesQueryHandler : IRequestHandler<GetDriveTypesQuery, IEnumerable<DictionaryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetDriveTypesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DictionaryDto>> Handle(GetDriveTypesQuery request, CancellationToken cancellationToken)
        {
            return await _context.DriveTypes
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new DictionaryDto { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken);
        }
    }
}


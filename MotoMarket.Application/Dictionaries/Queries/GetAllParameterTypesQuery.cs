using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Queries
{
    public record GetAllParameterTypesQuery(bool IncludeInactive = false) : IRequest<IEnumerable<ParameterTypeDto>>;

    public class GetAllParameterTypesQueryHandler : IRequestHandler<GetAllParameterTypesQuery, IEnumerable<ParameterTypeDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetAllParameterTypesQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<ParameterTypeDto>> Handle(GetAllParameterTypesQuery request, CancellationToken token)
        {
            var query = _context.VehicleParameterTypes.AsNoTracking().AsQueryable();
            if (!request.IncludeInactive) query = query.Where(x => x.IsActive);

            return await query.OrderBy(x => x.Category).ThenBy(x => x.Name)
                .Select(x => new ParameterTypeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Unit = x.Unit,
                    InputType = x.InputType,
                    Category = x.Category,
                    IsRequired = x.IsRequired,
                    IsActive = x.IsActive
                })
                .ToListAsync(token);
        }
    }
}


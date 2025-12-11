using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Dictionaries.Queries
{
    // Zapytanie
    public record GetParametersQuery : IRequest<IEnumerable<ParameterTypeDto>>;

    // Handler
    public class GetParametersQueryHandler : IRequestHandler<GetParametersQuery, IEnumerable<ParameterTypeDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetParametersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ParameterTypeDto>> Handle(GetParametersQuery request, CancellationToken cancellationToken)
        {
            return await _context.VehicleParameterTypes
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Category) 
                .ThenBy(x => x.Name)
                .Select(x => new ParameterTypeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Unit = x.Unit,
                    InputType = x.InputType
                })
                .ToListAsync(cancellationToken);
        }
    }
}
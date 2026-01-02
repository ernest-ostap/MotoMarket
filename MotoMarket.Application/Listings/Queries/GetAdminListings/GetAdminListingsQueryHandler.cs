using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Queries.GetAdminListings
{
    public class GetAdminListingsQueryHandler : IRequestHandler<GetAdminListingsQuery, IEnumerable<AdminListingDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAdminListingsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminListingDto>> Handle(GetAdminListingsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Listings // Zmień na nazwę Twojej tabeli z ogłoszeniami (np. Vehicles lub Listings)
                .AsNoTracking()
                .Include(x => x.Brand).ThenInclude(m => m.VehicleModels) // Żeby pobrać nazwę auta
                .OrderByDescending(x => x.CreatedAt) // Najnowsze na górze
                .Select(x => new AdminListingDto
                {
                    Id = x.Id,
                    Title = x.Title, 
                    Price = x.Price,
                    CreatedAt = x.CreatedAt,
                    OwnerId = x.UserId, 
                    Status= x.Status.ToString(),
                    BanReason = x.BanReason,
                    BrandModel = $"{x.Brand.Name} {x.Model.Name}"
                })
                .ToListAsync(cancellationToken);
        }
    }
}

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
            var query = from l in _context.Listings.AsNoTracking()
                        join u in _context.Users on l.UserId equals u.Id into userJoin
                        from u in userJoin.DefaultIfEmpty()
                        join b in _context.VehicleBrands on l.BrandId equals b.Id
                        join m in _context.VehicleModels on l.ModelId equals m.Id
                        orderby l.CreatedAt descending
                        select new AdminListingDto
                        {
                            Id = l.Id,
                            Title = l.Title,
                            Price = l.Price,
                            CreatedAt = l.CreatedAt,
                            OwnerId = l.UserId,
                            OwnerEmail = u != null ? u.Email : null,
                            Status = l.Status.ToString(),
                            BanReason = l.BanReason,
                            BrandModel = b.Name + " " + m.Name
                        };
            return await query.ToListAsync(cancellationToken);
        }
    }
}

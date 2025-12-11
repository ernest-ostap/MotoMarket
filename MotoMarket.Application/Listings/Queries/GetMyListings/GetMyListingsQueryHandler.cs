using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Application.Listings.Queries.GetAllListings;
using MotoMarket.Domain.Entities.Listings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Queries.GetMyListings
{
    public class GetMyListingsQueryHandler : IRequestHandler<GetMyListingsQuery, IEnumerable<ListingDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService; // Potrzebujemy wiedzieć KIM jesteśmy

        public GetMyListingsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ListingDto>> Handle(GetMyListingsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return new List<ListingDto>(); // Lub rzuć UnauthorizedAccessException
            }

            return await _context.Listings
                .AsNoTracking()
                .Where(x => x.UserId == currentUserId) // <--- KLUCZOWE FILTROWANIE
                .Include(x => x.Brand)
                .Include(x => x.Model)
                .Include(x => x.Photos)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ListingDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price,
                    LocationCity = x.LocationCity,
                    ProductionYear = x.ProductionYear,
                    Mileage = x.Mileage,
                    Status = (int)x.Status,
                    CreatedAt = x.CreatedAt,
                    ExpiresAt = x.ExpiresAt,
                    BrandName = x.Brand.Name,
                    ModelName = x.Model.Name,
                    MainPhotoUrl = x.Photos.Where(p => p.IsMain).Select(p => p.Url).FirstOrDefault() ?? string.Empty,
                    // Reszta pól opcjonalna na liście
                })
                .ToListAsync(cancellationToken);
        }
    }
}

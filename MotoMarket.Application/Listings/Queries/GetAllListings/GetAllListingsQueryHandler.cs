using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces; // Tutaj jest IApplicationDbContext
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Listings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Queries.GetAllListings
{
    public class GetAllListingsQueryHandler : IRequestHandler<GetAllListingsQuery, IEnumerable<ListingDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetAllListingsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ListingDto>> Handle(GetAllListingsQuery request, CancellationToken cancellationToken)
        {
            // Budowanie zapytania o ogłoszenia 
            var query = _context.Listings
                .AsNoTracking()
                .Where(x => x.Status == Domain.Entities.Listings.ListingStatus.Active)
                .Include(x => x.Brand)
                .Include(x => x.Model)
                .Include(x => x.Photos)
                .AsQueryable();

            // Filtry
            if (!string.IsNullOrWhiteSpace(request.SearchCallback))
                query = query.Where(x => x.Title.Contains(request.SearchCallback));

            if (request.BrandId.HasValue)
                query = query.Where(x => x.BrandId == request.BrandId.Value);

            if (request.ModelId.HasValue)
                query = query.Where(x => x.ModelId == request.ModelId.Value);

            if (request.PriceMin.HasValue)
                query = query.Where(x => x.Price >= request.PriceMin.Value);

            if (request.PriceMax.HasValue)
                query = query.Where(x => x.Price <= request.PriceMax.Value);

            if (request.YearMin.HasValue)
                query = query.Where(x => x.ProductionYear >= request.YearMin.Value);

            if (request.YearMax.HasValue)
                query = query.Where(x => x.ProductionYear <= request.YearMax.Value);

            // Sortowanie
            query = request.SortBy switch
            {
                "price_asc" => query.OrderBy(x => x.Price),
                "price_desc" => query.OrderByDescending(x => x.Price),
                "mileage_asc" => query.OrderBy(x => x.Mileage),
                "mileage_desc" => query.OrderByDescending(x => x.Mileage),
                "oldest" => query.OrderBy(x => x.ProductionYear),
                _ => query.OrderByDescending(x => x.CreatedAt)
            };

            // Wykonanie zapytania o ogłoszenia
            var listings = await query
                .Select(x => new ListingDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price,
                    LocationCity = x.LocationCity,
                    ProductionYear = x.ProductionYear,
                    Mileage = x.Mileage,
                    BrandName = x.Brand.Name,
                    ModelName = x.Model.Name,
                    MainPhotoUrl = x.Photos
                        .Where(p => p.IsMain)
                        .Select(p => p.Url)
                        .FirstOrDefault() ?? "",
                    IsFavorite = false //DOMYŚLNIE FALSE
                })
                .ToListAsync(cancellationToken);


            // Sprawdzanie Ulubionych 
            var userId = _currentUserService.UserId;

            // Jeśli użytkownik jest zalogowany...
            if (!string.IsNullOrEmpty(userId))
            {
                // ...pobieramy ID ogłoszeń, które ma w ulubionych
                var favoriteIds = await _context.UserFavorites
                    .AsNoTracking()
                    .Where(x => x.UserId == userId)
                    .Select(x => x.ListingId)
                    .ToListAsync(cancellationToken);

                // ...i aktualizujemy flagę w liście wynikowej
                if (favoriteIds.Any())
                {
                    foreach (var listing in listings)
                    {
                        if (favoriteIds.Contains(listing.Id))
                        {
                            listing.IsFavorite = true;
                        }
                    }
                }
            }

            return listings;
        }
    }
}
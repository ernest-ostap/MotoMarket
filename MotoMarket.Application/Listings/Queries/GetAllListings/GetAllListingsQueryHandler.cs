using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Listings;

namespace MotoMarket.Application.Listings.Queries.GetAllListings
{
    public class GetAllListingsQueryHandler : IRequestHandler<GetAllListingsQuery, IEnumerable<ListingDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllListingsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ListingDto>> Handle(GetAllListingsQuery request, CancellationToken cancellationToken)
        {
            // 1. Zaczynamy budować zapytanie (JESZCZE NIE WYSŁANE DO BAZY)
            var query = _context.Listings
                .AsNoTracking()
                .Where(x => x.Status == Domain.Entities.Listings.ListingStatus.Active) // Tylko aktywne
                .Include(x => x.Brand)
                .Include(x => x.Model)
                .Include(x => x.Photos)
                .AsQueryable(); // Ważne: rzutujemy na IQueryable, żeby doklejać warunki

            // 2. Doklejamy filtry dynamicznie
            if (!string.IsNullOrWhiteSpace(request.SearchCallback))
            {
                query = query.Where(x => x.Title.Contains(request.SearchCallback));
            }

            if (request.BrandId.HasValue)
            {
                query = query.Where(x => x.BrandId == request.BrandId.Value);
            }

            if (request.ModelId.HasValue)
            {
                query = query.Where(x => x.ModelId == request.ModelId.Value);
            }

            if (request.PriceMin.HasValue)
            {
                query = query.Where(x => x.Price >= request.PriceMin.Value);
            }

            if (request.PriceMax.HasValue)
            {
                query = query.Where(x => x.Price <= request.PriceMax.Value);
            }

            if (request.YearMin.HasValue)
            {
                query = query.Where(x => x.ProductionYear >= request.YearMin.Value);
            }

            // 3. Sortowanie (prosta implementacja)
            query = request.SortBy switch
            {
                "price_asc" => query.OrderBy(x => x.Price),
                "price_desc" => query.OrderByDescending(x => x.Price),
                "mileage_asc" => query.OrderBy(x => x.Mileage),
                "mileage_desc" => query.OrderByDescending(x => x.Mileage),
                "oldest" => query.OrderBy(x => x.ProductionYear),
                _ => query.OrderByDescending(x => x.CreatedAt) // Domyślne: najnowsze
            };

            // 4. Projekcja i Wykonanie (Dopiero tu leci SQL do bazy)
            return await query
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
                    // Zdjęcie główne
                    MainPhotoUrl = x.Photos
                        .Where(p => p.IsMain)
                        .Select(p => p.Url)
                        .FirstOrDefault() ?? "" // Jesli null to pusty string
                })
                .ToListAsync(cancellationToken);
        }
    }
}

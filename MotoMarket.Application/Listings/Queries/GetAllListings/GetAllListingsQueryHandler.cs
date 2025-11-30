using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces;

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
            return await _context.Listings
                .AsNoTracking()
                // Przy uzyciu .select() zazwyczaj EF sam robi sobie joiny,
                // dla jasnosci i przejrzystosci zostawiam include'y
                .Include(x => x.Brand)
                .Include(x => x.Model)
                .Include(x => x.VehicleCategory)
                .Include(x => x.FuelType)
                .Include(x => x.GearboxType)
                .Include(x => x.DriveType)
                .Include(x => x.BodyType)
                .Include(x => x.Photos)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ListingDto
                {
                    // Mapowanie 1:1
                    Id = x.Id,
                    UserId = x.UserId,
                    Title = x.Title,
                    Description = x.Description,
                    Price = x.Price,
                    VIN = x.VIN,
                    ProductionYear = x.ProductionYear,
                    Mileage = x.Mileage,
                    LocationCity = x.LocationCity,
                    LocationRegion = x.LocationRegion,
                    Status = (int)x.Status, // Rzutowanie enuma na int
                    CreatedAt = x.CreatedAt,
                    ExpiresAt = x.ExpiresAt,

                    // Wyciąganie nazw ze słowników
                    BrandName = x.Brand.Name,
                    ModelName = x.Model.Name,
                    VehicleCategoryName = x.VehicleCategory.Name,
                    FuelTypeName = x.FuelType.Name,
                    GearboxTypeName = x.GearboxType.Name,
                    DriveTypeName = x.DriveType.Name,
                    BodyTypeName = x.BodyType.Name,

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

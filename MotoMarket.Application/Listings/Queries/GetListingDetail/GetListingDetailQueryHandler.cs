using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Listings.Queries.GetAllListings; // Do podstawowego mapowania

namespace MotoMarket.Application.Listings.Queries.GetListingDetail
{
    public class GetListingDetailQueryHandler : IRequestHandler<GetListingDetailQuery, ListingDetailDto>
    {
        private readonly IApplicationDbContext _context;

        public GetListingDetailQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ListingDetailDto> Handle(GetListingDetailQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Listings
                .AsNoTracking()
                .Include(x => x.Brand)
                .Include(x => x.Model)
                .Include(x => x.VehicleCategory)
                .Include(x => x.FuelType)
                .Include(x => x.GearboxType)
                .Include(x => x.DriveType)
                .Include(x => x.BodyType)
                .Include(x => x.Photos)
                .Include(x => x.Features) // Pobieramy relację M:N
                    .ThenInclude(f => f.Feature) // I nazwę cechy
                .Include(x => x.ListingParameters)
                    .ThenInclude(p => p.ParameterType)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                // Tutaj w przyszłości rzucimy wyjątek NotFoundException
                return null;
            }

            // Mapowanie ręczne 
            return new ListingDetailDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Title = entity.Title,
                Description = entity.Description,
                Price = entity.Price,
                VIN = entity.VIN,
                ProductionYear = entity.ProductionYear,
                Mileage = entity.Mileage,
                LocationCity = entity.LocationCity,
                LocationRegion = entity.LocationRegion,
                Status = (int)entity.Status,
                CreatedAt = entity.CreatedAt,
                ExpiresAt = entity.ExpiresAt,

                BrandName = entity.Brand.Name,
                ModelName = entity.Model.Name,
                VehicleCategoryName = entity.VehicleCategory.Name,
                FuelTypeName = entity.FuelType.Name,
                GearboxTypeName = entity.GearboxType.Name,
                DriveTypeName = entity.DriveType.Name,
                BodyTypeName = entity.BodyType.Name,

                MainPhotoUrl = entity.Photos.FirstOrDefault(p => p.IsMain)?.Url ?? "",

                // --- To są te nowe detale ---
                PhotoUrls = entity.Photos.Select(p => p.Url).ToList(),

                Features = entity.Features.Select(f => f.Feature.Name).ToList(),

                Parameters = entity.ListingParameters.Select(p => new ListingParameterDto
                {
                    Name = p.ParameterType.Name,
                    Value = p.Value,
                    Unit = p.ParameterType.Unit
                }).ToList()
            };
        }
    }
}

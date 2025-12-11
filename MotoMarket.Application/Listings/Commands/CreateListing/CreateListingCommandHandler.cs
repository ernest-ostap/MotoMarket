using MediatR;
using MotoMarket.Domain.Entities;
using MotoMarket.Domain.Entities.Listings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Application.Common.Interfaces.Identity;

namespace MotoMarket.Application.Listings.Commands.CreateListing
{
    public class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService; 

        public CreateListingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(CreateListingCommand request, CancellationToken cancellationToken)
        {
            // 1. Tworzymy nową encję Listing na podstawie danych z komendy
            var entity = new Listing
            {
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,

                BrandId = request.BrandId,
                ModelId = request.ModelId,
                VehicleCategoryId = request.VehicleCategoryId,
                FuelTypeId = request.FuelTypeId,
                GearboxTypeId = request.GearboxTypeId,
                DriveTypeId = request.DriveTypeId,
                BodyTypeId = request.BodyTypeId,

                VIN = request.VIN,
                ProductionYear = request.ProductionYear,
                Mileage = request.Mileage,
                LocationCity = request.LocationCity,
                LocationRegion = request.LocationRegion,

                // Pobieramy ID z tokena. Jeśli tokena brak (co nie powinno się zdarzyć przez [Authorize]), dajemy fallback.
                UserId = _currentUserService.UserId ?? "Anonymous-User",

                Status = ListingStatus.Active, // Domyślnie aktywne
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30), // Ważne przez 30 dni

                Features = request.SelectedFeatureIds.Select(fid => new ListingFeature
                {
                    FeatureId = fid
                }).ToList(),

                ListingParameters = request.Parameters
                    .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                    .Select(kvp => new ListingParameter
                    {
                        ParameterTypeId = kvp.Key,
                        Value = kvp.Value
                    }).ToList(),

                Photos = BuildPhotos(request)

            };

            // 2. Dodajemy do bazy
            _context.Listings.Add(entity);

            // 3. Zapisujemy zmiany (leci INSERT do SQL)
            await _context.SaveChangesAsync(cancellationToken);

            // 4. Zwracamy ID nowego ogłoszenia
            return entity.Id;
        }

        private List<ListingPhoto> BuildPhotos(CreateListingCommand request)
        {
            if (request.Photos.Any())
            {
                return request.Photos
                    .OrderBy(p => p.SortOrder)
                    .Select(p => new ListingPhoto
                    {
                        Url = p.Url,
                        IsMain = p.IsMain,
                        SortOrder = p.SortOrder
                    })
                    .ToList();
            }

            // Fallback: stara ścieżka bez metadanych
            return request.PhotoUrls
                .Select((url, idx) => new ListingPhoto
                {
                    Url = url,
                    IsMain = idx == 0,
                    SortOrder = idx
                })
                .ToList();
        }
    }
}
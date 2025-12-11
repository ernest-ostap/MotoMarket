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

                Photos = request.PhotoUrls.Select(url => new ListingPhoto
                {
                    Url = url,
                    IsMain = request.PhotoUrls.First() == url // Pierwsze zdjęcie jest główne
                }).ToList()
            };

            // 2. Dodajemy do bazy
            _context.Listings.Add(entity);

            // 3. Zapisujemy zmiany (leci INSERT do SQL)
            await _context.SaveChangesAsync(cancellationToken);

            // 4. Zwracamy ID nowego ogłoszenia
            return entity.Id;
        }
    }
}
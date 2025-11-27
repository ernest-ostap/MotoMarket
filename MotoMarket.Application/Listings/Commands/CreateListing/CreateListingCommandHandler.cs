using MediatR;
using MotoMarket.Domain.Entities;
using MotoMarket.Domain.Entities.Listings;
using MotoMarket.Application.Common.Interfaces; // tu mamy IApplicationDbContext - obejscie na cykliczne zaleznosci
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Commands.CreateListing
{
    public class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateListingCommandHandler(IApplicationDbContext context)
        {
            _context = context;
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

                Status = ListingStatus.Active, // Domyślnie aktywne
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30), // Ważne przez 30 dni

                // UWAGA: Tutaj w przyszłości wstawimy prawdziwe ID zalogowanego użytkownika
                UserId = "SYSTEM-TEMP-USER"
            };

            // 2. Dodajemy do bazy
            _context.Listings.Add(entity);

            // 3. Zapisujemy zmiany (to tutaj leci INSERT do SQL)
            await _context.SaveChangesAsync(cancellationToken);

            // 4. Zwracamy ID nowego ogłoszenia
            return entity.Id;
        }
    }
}
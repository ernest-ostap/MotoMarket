using MediatR;
using MotoMarket.Application.Common.Exceptions;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Listings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Commands.UpdateListing
{
    public class UpdateListingCommandHandler : IRequestHandler<UpdateListingCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateListingCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateListingCommand request, CancellationToken cancellationToken)
        {
            // 1. pobieranie encji z bazy
            var entity = await _context.Listings
                .FindAsync(new object[] { request.Id }, cancellationToken);

            // 2. sprawdzanie czy istnieje, jeżeli nie to obsługa błędu
            if (entity == null)
            {
                //zastosowanie wyjątku NotFoundException z parametrem nameof(Listing) oraz request.Id,
                //bardziej profesjonalne niż pusty error w api
                throw new NotFoundException(nameof(Listing), request.Id);
            }

            // 3. Aktualizujemy pola (Mapowanie)
            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.Price = request.Price;

            entity.BrandId = request.BrandId;
            entity.ModelId = request.ModelId;
            entity.VehicleCategoryId = request.VehicleCategoryId;
            entity.FuelTypeId = request.FuelTypeId;
            entity.GearboxTypeId = request.GearboxTypeId;
            entity.DriveTypeId = request.DriveTypeId;
            entity.BodyTypeId = request.BodyTypeId;

            entity.VIN = request.VIN;
            entity.ProductionYear = request.ProductionYear;
            entity.Mileage = request.Mileage;
            entity.LocationCity = request.LocationCity;
            entity.LocationRegion = request.LocationRegion;

            // aktualizacja daty modyfikacji
            entity.UpdatedAt = DateTime.UtcNow;

            // 4. zapis zmian w bazie
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

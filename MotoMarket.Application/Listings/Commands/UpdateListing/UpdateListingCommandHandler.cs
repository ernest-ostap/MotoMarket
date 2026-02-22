using MediatR;
using MotoMarket.Application.Common.Exceptions;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Listings;
using MotoMarket.Application.Listings.Commands.CreateListing;
using Microsoft.EntityFrameworkCore;
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
            // 1. Get entity by id
            var entity = await _context.Listings
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
                throw new NotFoundException(nameof(Listing), request.Id);

            // 2. Map and update fields
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

            // Photos: replace if provided
            if (request.Photos.Any() || request.PhotoUrls.Any())
            {
                _context.ListingPhotos.RemoveRange(_context.ListingPhotos.Where(p => p.ListingId == entity.Id));

                var newPhotos = BuildPhotos(request);
                foreach (var photo in newPhotos)
                {
                    photo.ListingId = entity.Id;
                }
                await _context.ListingPhotos.AddRangeAsync(newPhotos, cancellationToken);
            }

            // Features: clear and replace
            _context.ListingFeatures.RemoveRange(_context.ListingFeatures.Where(f => f.ListingId == entity.Id));
            var newFeatures = request.SelectedFeatureIds
                .Select(fid => new ListingFeature { ListingId = entity.Id, FeatureId = fid })
                .ToList();
            if (newFeatures.Any())
            {
                await _context.ListingFeatures.AddRangeAsync(newFeatures, cancellationToken);
            }

            // ListingParameters: clear and replace (non-empty values only)
            _context.ListingParameters.RemoveRange(_context.ListingParameters.Where(p => p.ListingId == entity.Id));
            var newParams = request.Parameters
                .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
                .Select(kvp => new ListingParameter
                {
                    ListingId = entity.Id,
                    ParameterTypeId = kvp.Key,
                    Value = kvp.Value
                })
                .ToList();
            if (newParams.Any())
            {
                await _context.ListingParameters.AddRangeAsync(newParams, cancellationToken);
            }

            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        private List<ListingPhoto> BuildPhotos(UpdateListingCommand request)
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

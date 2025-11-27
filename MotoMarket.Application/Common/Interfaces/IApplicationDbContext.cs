using Microsoft.EntityFrameworkCore;
using MotoMarket.Domain.Entities;
using MotoMarket.Domain.Entities.Configuration;
using MotoMarket.Domain.Entities.Listings;
using MotoMarket.Domain.Entities.System;
using MotoMarket.Domain.Entities.Vehicles;
using DriveType = MotoMarket.Domain.Entities.Vehicles.DriveType;

namespace MotoMarket.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        // --- Ogłoszenia ---
        DbSet<Listing> Listings { get; }
        DbSet<ListingPhoto> ListingPhotos { get; }
        DbSet<ListingFeature> ListingFeatures { get; }
        DbSet<ListingParameter> ListingParameters { get; }
        DbSet<ListingView> ListingViews { get; }

        // --- Słowniki (Pojazdy) ---
        DbSet<VehicleBrand> VehicleBrands { get; }
        DbSet<VehicleModel> VehicleModels { get; }
        DbSet<VehicleCategory> VehicleCategories { get; }
        DbSet<FuelType> FuelTypes { get; }
        DbSet<GearboxType> GearboxTypes { get; }
        DbSet<DriveType> DriveTypes { get; }
        DbSet<BodyType> BodyTypes { get; }
        DbSet<VehicleFeature> VehicleFeatures { get; }
        DbSet<VehicleParameterType> VehicleParameterTypes { get; }

        // --- System & Logi ---
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<ErrorLog> ErrorLogs { get; }

        // --- Konfiguracja ---
        DbSet<AdminSetting> AdminSettings { get; }
        DbSet<PageContent> PageContents { get; }

        // --- Metoda zapisu ---
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
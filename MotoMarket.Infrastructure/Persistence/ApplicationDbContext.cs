using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Domain.Entities;
using MotoMarket.Domain.Entities.Configuration;
using MotoMarket.Domain.Entities.Listings;
using MotoMarket.Domain.Entities.System;
using MotoMarket.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveType = MotoMarket.Domain.Entities.Vehicles.DriveType;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Infrastructure.Persistence
{
    // dziedziczymy po IdentityDbContext, żeby mieć gotowe tabele Users, Roles, Logins itp.
    // dziedziczymy też po IApplicationDbContext, żeby wstrzykiwać kontekst przez interfejs - rozwiązanie na cykliczne zależności ;)
    public class ApplicationDbContext : IdentityDbContext <ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // --- Ogłoszenia ---
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingPhoto> ListingPhotos { get; set; }
        public DbSet<ListingFeature> ListingFeatures { get; set; }
        public DbSet<ListingParameter> ListingParameters { get; set; }
        public DbSet<ListingView> ListingViews { get; set; }

        // --- System & Logs ---
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        // --- Configuration / CMS ---
        public DbSet<AdminSetting> AdminSettings { get; set; }
        public DbSet<PageContent> PageContents { get; set; }

        // --- Słowniki (Pojazdy) ---
        public DbSet<VehicleBrand> VehicleBrands { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
        public DbSet<VehicleCategory> VehicleCategories { get; set; }
        public DbSet<FuelType> FuelTypes { get; set; }
        public DbSet<GearboxType> GearboxTypes { get; set; }
        public DbSet<DriveType> DriveTypes { get; set; }
        public DbSet<BodyType> BodyTypes { get; set; }

        public DbSet<VehicleFeature> VehicleFeatures { get; set; }
        public DbSet<VehicleParameterType> VehicleParameterTypes { get; set; }

        public DbSet<UserFavorite> UserFavorites { get; set; }

        // Tutaj konfigurujemy szczegóły, np. precyzję ceny
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // To musi być, żeby Identity zadziałało!

            // Konfiguracja precyzji dla Ceny (wymagane w MS SQL dla typu decimal)
            builder.Entity<Listing>()
                .Property(l => l.Price)
                .HasColumnType("decimal(18,2)");

            // Relacja jeden-do-wielu: Marka -> Modele
            builder.Entity<VehicleModel>()
                .HasOne(m => m.VehicleBrand)
                .WithMany(b => b.VehicleModels)
                .HasForeignKey(m => m.VehicleBrandId)
                .OnDelete(DeleteBehavior.Restrict); // Nie chcemy usuwać modeli jak usuniemy markę (bezpiecznik)

            // Relacja Ogłoszenie -> Marka, Model itd.
            // Ustawiamy DeleteBehavior.Restrict, żeby usunięcie marki z bazy nie kazało kaskadowo usuwać ogłoszeń
            builder.Entity<Listing>()
                .HasOne(l => l.Brand)
                .WithMany()
                .HasForeignKey(l => l.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Listing>()
                .HasOne(l => l.Model)
                .WithMany()
                .HasForeignKey(l => l.ModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserFavorite>()
                .HasKey(x => new { x.UserId, x.ListingId }); // Klucz złożony

            builder.Entity<UserFavorite>()
                .HasOne(x => x.Listing)
                .WithMany() 
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reszta relacji ListingFeature, ListingPhoto itp. z automatu zadziała dobrze (Cascade delete jest tam OK)
        }
    }
}

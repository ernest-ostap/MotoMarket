using Microsoft.EntityFrameworkCore;
using MotoMarket.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveType = MotoMarket.Domain.Entities.Vehicles.DriveType;

namespace MotoMarket.Infrastructure.Persistence
{
    public class ApplicationDbContextSeeder
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            //sprawdzenie czy baza istnieje
            try
            {
                if(_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas migracji bazy danych.", ex);
            }

            await SeedVehicleDictionaryAsync();
        }

        private async Task SeedVehicleDictionaryAsync()
        {
            // 1. MARKI (Brands) - sprawdzamy czy są jakiekolwiek
            if (!_context.VehicleBrands.Any())
            {
                var brands = new List<VehicleBrand>
                {
                    new() { Name = "Audi", IsActive = true },
                    new() { Name = "BMW", IsActive = true },
                    new() { Name = "Mercedes-Benz", IsActive = true },
                    new() { Name = "Volkswagen", IsActive = true }, 
                    new() { Name = "Toyota", IsActive = true },
                    new() { Name = "Ford", IsActive = true },
                    new() {
                        Name = "Opel",
                        IsActive = true,
                        VehicleModels = new List<VehicleModel> { new() { Name = "Astra", IsActive = true }, new() { Name = "Insignia", IsActive = true } }
                    },
                    new() { Name = "Volvo", IsActive = true }
                };

                _context.VehicleBrands.AddRange(brands);
            }

            // 2. TYPY PALIWA (FuelTypes)
            if (!_context.FuelTypes.Any())
            {
                var fuels = new List<FuelType>
                {
                    new() { Name = "Benzyna", IsActive = true },
                    new() { Name = "Diesel", IsActive = true },
                    new() { Name = "Benzyna + LPG", IsActive = true },
                    new() { Name = "Hybryda", IsActive = true },
                    new() { Name = "Elektryczny", IsActive = true }
                };
                _context.FuelTypes.AddRange(fuels);
            }

            // 3. SKRZYNIE BIEGÓW
            if (!_context.GearboxTypes.Any())
            {
                _context.GearboxTypes.AddRange(new List<GearboxType>
                {
                    new() { Name = "Manualna", IsActive = true },
                    new() { Name = "Automatyczna", IsActive = true },
                    new() { Name = "CVT", IsActive = true }
                });
            }

            // 4. NAPĘDY
            if (!_context.DriveTypes.Any())
            {
                _context.DriveTypes.AddRange(new List<DriveType>
                {
                    new() { Name = "Na przednie koła (FWD)", IsActive = true },
                    new() { Name = "Na tylne koła (RWD)", IsActive = true },
                    new() { Name = "4x4 (AWD)", IsActive = true }
                });
            }

            // 5. NADWOZIA
            if (!_context.BodyTypes.Any())
            {
                _context.BodyTypes.AddRange(new List<BodyType>
                {
                    new() { Name = "Sedan", SortOrder = 1, IsActive = true },
                    new() { Name = "Kombi", SortOrder = 2, IsActive = true },
                    new() { Name = "SUV", SortOrder = 3, IsActive = true },
                    new() { Name = "Hatchback", SortOrder = 4, IsActive = true },
                    new() { Name = "Coupe", SortOrder = 5, IsActive = true },
                    new() { Name = "Cabrio", SortOrder = 6, IsActive = true }
                });
            }

            // 6. KATEGORIE POJAZDÓW
            if (!_context.VehicleCategories.Any())
            {
                _context.VehicleCategories.AddRange(new List<VehicleCategory>
                {
                    new() { Name = "Osobowe", IsActive = true },
                    new() { Name = "Dostawcze", IsActive = true },
                    new() { Name = "Motocykle", IsActive = true }
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}

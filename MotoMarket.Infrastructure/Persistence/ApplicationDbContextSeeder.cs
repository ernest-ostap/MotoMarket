using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Domain.Entities;
using MotoMarket.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriveType = MotoMarket.Domain.Entities.Vehicles.DriveType;

namespace MotoMarket.Infrastructure.Persistence
{
    public class ApplicationDbContextSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationDbContextSeeder(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            // Sprawdzenie czy baza istnieje i migracja
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas migracji bazy danych.", ex);
            }

            await SeedVehicleDictionaryAsync();
            // ZMIANA 4: Wywołujemy metodę seedowania userów!
            await SeedUsersAsync();
        }

        private async Task SeedVehicleDictionaryAsync()
        {
            // 1. MARKI (Brands)
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

            // 7. CECHY (Wyposażenie)
            if (!_context.VehicleFeatures.Any())
            {
                var features = new List<VehicleFeature>
                {
                    // Bezpieczeństwo
                    new() { Name = "ABS", GroupName = "Bezpieczeństwo", SortOrder = 1, IsActive = true },
                    new() { Name = "ESP", GroupName = "Bezpieczeństwo", SortOrder = 2, IsActive = true },
                    new() { Name = "Poduszka powietrzna kierowcy", GroupName = "Bezpieczeństwo", SortOrder = 3, IsActive = true },
            
                    // Komfort
                    new() { Name = "Klimatyzacja automatyczna", GroupName = "Komfort", SortOrder = 10, IsActive = true },
                    new() { Name = "Podgrzewane fotele", GroupName = "Komfort", SortOrder = 11, IsActive = true },
                    new() { Name = "Tempomat", GroupName = "Komfort", SortOrder = 12, IsActive = true },
            
                    // Multimedia
                    new() { Name = "Bluetooth", GroupName = "Multimedia", SortOrder = 20, IsActive = true },
                    new() { Name = "Nawigacja GPS", GroupName = "Multimedia", SortOrder = 21, IsActive = true },
            
                    // Wygląd
                    new() { Name = "Alufelgi", GroupName = "Wygląd", SortOrder = 30, IsActive = true },
                    new() { Name = "Światła LED", GroupName = "Wygląd", SortOrder = 31, IsActive = true }
                };
                _context.VehicleFeatures.AddRange(features);
            }

            // 8. PARAMETRY TECHNICZNE
            if (!_context.VehicleParameterTypes.Any())
            {
                var paramsTypes = new List<VehicleParameterType>
                {
                    new() { Name = "Pojemność skokowa", Unit = "cm3", InputType = "number", Category = "Silnik", IsRequired = true, IsActive = true },
                    new() { Name = "Moc", Unit = "KM", InputType = "number", Category = "Silnik", IsRequired = true, IsActive = true },
                    new() { Name = "Liczba drzwi", Unit = "", InputType = "number", Category = "Nadwozie", IsRequired = false, IsActive = true },
                    new() { Name = "Liczba miejsc", Unit = "", InputType = "number", Category = "Nadwozie", IsRequired = false, IsActive = true },
                    new() { Name = "Kolor", Unit = "", InputType = "text", Category = "Ogólne", IsRequired = true, IsActive = true }
                };
                _context.VehicleParameterTypes.AddRange(paramsTypes);
            }

            await _context.SaveChangesAsync();
        }

        // Ta metoda teraz zadziała poprawnie
        public async Task SeedUsersAsync()
        {
            // 1. Tworzenie ROL (jeśli nie istnieją)
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            // 2. Tworzenie ADMINA (jeśli nie istnieje)
            // Używamy FindByEmailAsync zamiast LINQ na Users.All (bezpieczniej)
            var adminUser = await _userManager.FindByEmailAsync("admin@motomarket.pl");

            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@motomarket.pl",
                    Email = "admin@motomarket.pl",
                    FirstName = "Główny",
                    LastName = "Administrator",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(admin, "Admin123!"); 
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            else
            {
                // Jeśli admin istnieje, upewnij się, że ma rolę (przydatne jak dodajesz role do istniejącej bazy)
                if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
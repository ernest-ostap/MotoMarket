using Microsoft.AspNetCore.Mvc.Rendering; // Do SelectListItem
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MotoMarket.Web.Models.ViewModels
{
    public class CreateListingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany")]
        [Display(Name = "Tytuł ogłoszenia")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Opis jest wymagany")]
        [Display(Name = "Opis")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cena jest wymagana")]
        [Range(1, 10000000, ErrorMessage = "Cena musi być między 1 a 10 000 000")]
        [Display(Name = "Cena")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "VIN jest wymagany")]
        [Display(Name = "VIN")]
        public string VIN { get; set; } = string.Empty;

        [Display(Name = "Rok produkcji")]
        public int ProductionYear { get; set; }

        [Display(Name = "Przebieg (km)")]
        public int Mileage { get; set; }

        [Display(Name = "Miasto")]
        public string LocationCity { get; set; } = string.Empty;

        [Display(Name = "Województwo")]
        public string LocationRegion { get; set; } = string.Empty;

        // --- Pola wyboru (to co user zaznaczy) ---
        [Required]
        [Display(Name = "Marka")]
        public int BrandId { get; set; }

        [Required]
        [Display(Name = "Model")]
        public int ModelId { get; set; }

        [Required]
        [Display(Name = "Rodzaj nadwozia")]
        public int BodyTypeId { get; set; }

        [Required]
        [Display(Name = "Rodzaj napędu")]
        public int DriveTypeId { get; set; }

        [Required]
        [Display(Name = "Rodzaj paliwa")]
        public int FuelTypeId { get; set; }

        [Required]
        [Display(Name = "Skrzynia biegów")]
        public int GearboxTypeId { get; set; }

        [Required]
        [Display(Name = "Kategoria pojazdu")]
        public int VehicleCategoryId { get; set; }


        // --- Listy do Dropdownów ---
        public IEnumerable<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Models { get; set; } = new List<SelectListItem>(); // Na starcie puste!
        public IEnumerable<SelectListItem> BodyTypes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DriveTypes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> FuelTypes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> GearboxTypes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> VehicleCategories { get; set; } = new List<SelectListItem>();

    }
}
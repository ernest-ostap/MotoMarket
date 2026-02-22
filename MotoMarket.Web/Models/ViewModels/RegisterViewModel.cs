using System.ComponentModel.DataAnnotations;

namespace MotoMarket.Web.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        [MinLength(10, ErrorMessage = "Hasło musi mieć co najmniej 10 znaków.")]
        [RegularExpression("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^A-Za-z0-9]).{10,}",
            ErrorMessage = "Hasło musi zawierać co najmniej jedną małą literę, jedną wielką literę, jedną cyfrę oraz jeden znak specjalny (np. !, @, /).")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła nie są zgodne.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Imię jest wymagane.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        public string LastName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Nieprawidłowy numer telefonu.")]
        public string? PhoneNumber { get; set; }
    }
}

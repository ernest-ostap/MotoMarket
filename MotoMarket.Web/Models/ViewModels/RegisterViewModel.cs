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
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła nie są zgodne.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage ="Imię jest wymagane.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage ="Nazwisko jest wymagane.")]
        public string LastName { get; set; } = string.Empty;
    }
}

//testowy user
//testzWEB@web.pl
//testzWEB123!@ 
using System.ComponentModel.DataAnnotations;

namespace MotoMarket.Web.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(10, ErrorMessage = "Hasło musi mieć co najmniej 10 znaków.")]
        [RegularExpression("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^A-Za-z0-9]).{10,}",
            ErrorMessage = "Hasło musi zawierać co najmniej jedną małą literę, jedną wielką literę, jedną cyfrę oraz jeden znak specjalny (np. !, @, /).")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Hasła nie są zgodne.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}


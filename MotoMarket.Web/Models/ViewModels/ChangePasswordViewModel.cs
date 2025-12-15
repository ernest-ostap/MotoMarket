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
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Hasła nie są zgodne.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}


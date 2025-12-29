namespace MotoMarket.Web.Models.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // np. "User", "Admin"
        public bool IsBanned { get; set; } // Czy ma blokadę?
        public DateTime CreatedAt { get; set; } // Data rejestracji (opcjonalnie)
    }
}
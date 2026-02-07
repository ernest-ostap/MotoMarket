namespace MotoMarket.Web.Models.DTOs
{
    public class AdminListingDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string? OwnerEmail { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? BanReason { get; set; }
        public string BrandModel { get; set; } = string.Empty; 
    }
}
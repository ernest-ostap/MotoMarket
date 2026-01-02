using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Listings.Queries.GetAdminListings
{
    public class AdminListingDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OwnerId { get; set; } = string.Empty; // Żeby ewentualnie podlinkować do Usera
        public string Status { get; set; } = string.Empty;
        public string? BanReason { get; set; }
        public string BrandModel { get; set; } = string.Empty;
    }
}

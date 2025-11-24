using MotoMarket.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.Listings
{
    public class ListingView : BaseEntity
    {
        public int ListingId { get; set; }
        public Listing Listing { get; set; } = null!;

        // Może być null, bo niezalogowani też oglądają
        public string? UserId { get; set; }

        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty; // np. Chrome Mobile
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}

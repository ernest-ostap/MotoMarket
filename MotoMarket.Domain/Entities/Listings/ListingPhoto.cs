using MotoMarket.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.Listings
{
    public class ListingPhoto : BaseEntity
    {
        public int ListingId { get; set; }
        public Listing Listing { get; set; } = null!;

        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}

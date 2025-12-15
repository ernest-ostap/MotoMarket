using MotoMarket.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.Chat
{
    public class ChatMessage : BaseEntity
    {
        public string SenderId { get; set; } = string.Empty;
        public string RecipientId { get; set; } = string.Empty;

        public int? ListingId { get; set; }
        public Listings.Listing? Listing { get; set; }

        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

    }
}

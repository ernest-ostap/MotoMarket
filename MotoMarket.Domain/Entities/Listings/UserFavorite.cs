using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.Listings
{
    public class UserFavorite
    {
        public string UserId { get; set; } = string.Empty; // ID Usera (z Identity)

        public int ListingId { get; set; } // ID Ogłoszenia
        public Listing Listing { get; set; } = null!;
    }
}

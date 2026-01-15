using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Mobile.Models.Listings
{
    public enum ListingStatus
    {
        Draft = 0,
        Active = 1,
        Sold = 2,
        Archived = 3,
        Banned = 4
    }
}

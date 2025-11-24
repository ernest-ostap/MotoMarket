using MotoMarket.Domain.Common;
using MotoMarket.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.Listings
{
    public class ListingFeature : BaseEntity
    {
        public int ListingId { get; set; }
        public Listing Listing { get; set; } = null!;

        public int FeatureId { get; set; }
        public VehicleFeature Feature { get; set; } = null!;
    }
}

using MotoMarket.Domain.Common;
using MotoMarket.Domain.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.Listings
{
    public class ListingParameter : BaseEntity
    {
        public int ListingId { get; set; }
        public Listing Listing { get; set; } = null!;

        public int ParameterTypeId { get; set; }
        public VehicleParameterType ParameterType { get; set; } = null!;

        // W EAV wartość trzymamy zazwyczaj jako string i konwertujemy przy wyświetlaniu
        public string Value { get; set; } = string.Empty;
    }
}

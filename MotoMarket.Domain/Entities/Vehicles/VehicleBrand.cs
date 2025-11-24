using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.Vehicles
{
    public class VehicleBrand : Common.BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public ICollection<VehicleModel> VehicleModels { get; set; } = new List<VehicleModel>();

    }
}

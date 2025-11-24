using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.Vehicles
{
    public class VehicleModel : Common.BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int VehicleBrandId { get; set; }
        public VehicleBrand? VehicleBrand { get; set; }
        public bool IsActive { get; set; }
    }
}

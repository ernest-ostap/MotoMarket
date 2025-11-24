using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.Vehicles
{
    public class VehicleFeature : Common.BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}


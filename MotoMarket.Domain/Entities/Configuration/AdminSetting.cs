using MotoMarket.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.Configuration
{
    public class AdminSetting : BaseEntity
    {
        public string Key { get; set; } = string.Empty;   // Unikalny klucz, np. "MAX_PHOTOS_PER_LISTING"
        public string Value { get; set; } = string.Empty; // Wartość, np. "10"
        public string? Description { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

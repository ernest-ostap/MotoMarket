using MotoMarket.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.System
{
    public class AuditLog : BaseEntity
    {
        public string UserId { get; set; } = string.Empty; // ID użytkownika (lub "System" / "Anonymous")
        public string ActionType { get; set; } = string.Empty; // np. "Create", "Update"
        public string EntityName { get; set; } = string.Empty; // np. "Listing"
        public string EntityId { get; set; } = string.Empty;   // Zapisujemy jako string, bo ID mogą być int lub Guid

        // Dane jako JSON (string)
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

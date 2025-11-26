using MotoMarket.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.System
{
    public class ErrorLog : BaseEntity
    {
        public string Message { get; set; } = string.Empty;
        public string ExceptionType { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? Source { get; set; } // Klasa/metoda

        public string? Endpoint { get; set; } // URL requestu
        public string? RequestBody { get; set; } // Opcjonalny zrzut danych

        public string? UserId { get; set; } // Kto wywołał błąd (jeśli zalogowany)

        // Status naprawy
        public bool IsResolved { get; set; }
        public string? ResolvedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
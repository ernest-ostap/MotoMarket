using MotoMarket.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Domain.Entities.Configuration
{
    public class PageContent : BaseEntity
    {
        public string PageKey { get; set; } = string.Empty; // np. "HOMEPAGE_WELCOME_TEXT"
        public string Content { get; set; } = string.Empty; // HTML lub Markdown

        public DateTime LastModified { get; set; } = DateTime.UtcNow;
        public string? ModifiedBy { get; set; }
    }
}

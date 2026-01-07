using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.PageContent.Queries.GetAllPageContent
{
    public class PageContentDto
    {
        public int Id { get; set; }
        public string PageKey { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
    }
}

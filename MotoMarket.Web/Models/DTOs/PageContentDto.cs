namespace MotoMarket.Web.Models.DTOs
{
    public class PageContentDto
    {
        public int Id { get; set; }
        public string PageKey { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
    }
}

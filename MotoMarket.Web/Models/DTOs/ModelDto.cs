namespace MotoMarket.Web.Models.DTOs
{
    public class ModelDto : DictionaryDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
    }
}

namespace MotoMarket.Web.Models.DTOs
{
    public class ListingDetailDto : ListingDto
    {
        public IEnumerable<string> PhotoUrls { get; set; } = new List<string>();
        public IEnumerable<string> Features { get; set; } = new List<string>();
        public IEnumerable<ListingParametersDto> Parameters { get; set; } = new List<ListingParametersDto>();
    }

    public class ListingParametersDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }
}

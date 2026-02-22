namespace MotoMarket.Mobile.Models.Listings
{
    public class FeatureDto : DictionaryDto
    {
        public string GroupName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}

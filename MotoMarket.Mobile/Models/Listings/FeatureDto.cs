namespace MotoMarket.Mobile.Models.Listings
{
    public class FeatureDto : DictionaryDto
    {
        public string GroupName { get; set; } = string.Empty;
        
        // Czy feature jest zaznaczony
        public bool IsSelected { get; set; }
    }
}

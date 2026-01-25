namespace MotoMarket.Mobile.Models.Listings
{
    public class ParameterTypeDto : DictionaryDto
    {
        public string? Unit { get; set; }
        public string InputType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        
        // Wartość wprowadzona przez użytkownika
        public string Value { get; set; } = string.Empty;
    }
}

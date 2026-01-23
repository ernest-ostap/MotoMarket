namespace MotoMarket.Web.Models.Other
{
    public class PdfGenerationOptions
    {
        public int ListingId { get; set; } // Żeby wiedzieć co drukujemy
        public string CustomTitle { get; set; } // Parametryzacja tekstu
        public bool IncludePhotos { get; set; } // Zmiana układu
        public bool IncludeDescription { get; set; }
        public bool IncludeFeatures { get; set; } 
        public bool IncludePrice { get; set; }
    }
}
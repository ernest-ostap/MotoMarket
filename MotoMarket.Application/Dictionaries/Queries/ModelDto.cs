using MotoMarket.Application.Dictionaries.Queries;

namespace MotoMarket.Web.Models.DTOs
{
    public class ModelDto : DictionaryDto
    {
        public int BrandId { get; set; } // ID Marki (do edycji)
        public string BrandName { get; set; } = string.Empty; // Nazwa Marki (do wyświetlenia w tabeli)
    }
}
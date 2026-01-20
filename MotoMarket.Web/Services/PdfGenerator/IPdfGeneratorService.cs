using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Web.Services.PdfGenerator
{
    public interface IPdfGeneratorService
    {
        Task<byte[]> GenerateListingPdfAsync(ListingDetailDto model, List<string> sectionOrder);
    }
}

using MotoMarket.Web.Models.DTOs;
using MotoMarket.Web.Models.Other;

namespace MotoMarket.Web.Services.PdfGenerator
{
    public interface IPdfGeneratorService
    {
        Task<byte[]> GenerateListingPdfAsync(ListingDetailDto model, PdfGenerationOptions options);
    }
}

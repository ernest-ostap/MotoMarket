using MotoMarket.Web.Models.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MotoMarket.Web.Services.PdfGenerator
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PdfGeneratorService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<byte[]> GenerateListingPdfAsync(ListingDetailDto model, List<string> sectionOrder)
        {
            // Pobieramy główne zdjęcie (jeśli jest) jako bajty
            byte[] mainImageBytes = null;
            if (!string.IsNullOrEmpty(model.MainPhotoUrl))
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    mainImageBytes = await client.GetByteArrayAsync(model.MainPhotoUrl);
                }
                catch { /* Ignorujemy błędy pobierania zdjęcia, PDF wygeneruje się bez niego */ }
            }

            // Tworzenie dokumentu
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial")); // Ustawienie czcionki

                    page.Header().Element(c => ComposeHeader(c, model));

                    // TUTAJ DZIEJE SIĘ MAGIA "PRZESTAWIALNYCH BLOKÓW"
                    page.Content().Element(c => ComposeContent(c, model, mainImageBytes, sectionOrder));

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                        x.Span($" | Wygenerowano z MotoMarket: {DateTime.Now:dd.MM.yyyy}");
                    });
                });
            });

            return document.GeneratePdf();
        }

        // --- KOMPONENTY PDF ---

        void ComposeHeader(IContainer container, ListingDetailDto model)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"{model.BrandName} {model.ModelName}").FontSize(24).SemiBold().FontColor(Colors.Blue.Medium);
                    column.Item().Text(model.Title).FontSize(14).Italic();
                });

                row.ConstantItem(150).Column(column =>
                {
                    column.Item().AlignRight().Text($"{model.Price:N0} PLN").FontSize(24).Bold().FontColor(Colors.Red.Medium);
                    column.Item().AlignRight().Text($"{model.LocationCity}").FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });
        }

        void ComposeContent(IContainer container, ListingDetailDto model, byte[] imageBytes, List<string> sections)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(20);

                // Iterujemy po liście sekcji zdefiniowanej przez użytkownika/kontroler
                foreach (var section in sections)
                {
                    switch (section.ToLower())
                    {
                        case "photo":
                            if (imageBytes != null)
                                column.Item().Image(imageBytes).FitArea();
                            break;

                        case "specs":
                            column.Item().Element(c => ComposeSpecsTable(c, model));
                            break;

                        case "description":
                            column.Item().Column(c =>
                            {
                                c.Item().Text("Opis pojazdu").FontSize(16).SemiBold();
                                c.Item().Text(model.Description ?? "Brak opisu").Justify();
                            });
                            break;

                        case "features":
                            column.Item().Element(c => ComposeFeatures(c, model));
                            break;
                    }
                }
            });
        }

        void ComposeSpecsTable(IContainer container, ListingDetailDto model)
        {
            container.Table(table =>
            {
                // Definicja kolumn
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150);
                    columns.RelativeColumn();
                });

                // Style komórek
                static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                static IContainer HeaderStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Medium).PaddingVertical(5);

                table.Header(header =>
                {
                    header.Cell().Element(HeaderStyle).Text("Parametr").SemiBold();
                    header.Cell().Element(HeaderStyle).Text("Wartość").SemiBold();
                });

                // Wiersze
                void AddRow(string label, string value)
                {
                    table.Cell().Element(CellStyle).Text(label).FontColor(Colors.Grey.Darken2);
                    table.Cell().Element(CellStyle).Text(value).SemiBold();
                }

                AddRow("Rok produkcji", model.ProductionYear.ToString());
                AddRow("Przebieg", $"{model.Mileage} km");
                AddRow("Paliwo", model.FuelTypeName);
                AddRow("Skrzynia biegów", model.GearboxTypeName);
                AddRow("Nadwozie", model.BodyTypeName);
                if (!string.IsNullOrEmpty(model.VIN)) AddRow("VIN", model.VIN);

                // Parametry dynamiczne
                if (model.Parameters != null)
                {
                    foreach (var p in model.Parameters)
                    {
                        AddRow(p.Name, $"{p.Value} {p.Unit}");
                    }
                }
            });
        }

        void ComposeFeatures(IContainer container, ListingDetailDto model)
        {
            if (model.Features == null || !model.Features.Any()) return;

            container.Column(column =>
            {
                column.Item().Text("Wyposażenie").FontSize(16).SemiBold();

                column.Item().PaddingTop(5).Grid(grid =>
                {
                    grid.Columns(3); // 3 kolumny wyposażenia
                    grid.Spacing(5);

                    foreach (var feature in model.Features)
                    {
                        grid.Item().Background(Colors.Grey.Lighten4).Padding(5).Text($"• {feature}").FontSize(9);
                    }
                });
            });
        }
    }
}

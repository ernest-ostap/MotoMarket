namespace MotoMarket.Web.Models.DTOs
{
    public class ListingDetailDto : ListingDto
    {
        // ID właściwości potrzebne do edycji
        public int BrandId { get; set; }
        public int ModelId { get; set; }
        public int FuelTypeId { get; set; }
        public int GearboxTypeId { get; set; }
        public int BodyTypeId { get; set; }
        public int DriveTypeId { get; set; }
        public int VehicleCategoryId { get; set; }

        public IEnumerable<string> PhotoUrls { get; set; } = new List<string>();
        public IEnumerable<string> Features { get; set; } = new List<string>();

        // Do edycji
        public IEnumerable<int> FeatureIds { get; set; } = new List<int>();
        public IEnumerable<ListingParametersDto> Parameters { get; set; } = new List<ListingParametersDto>();

        public string SellerPhone { get; set; } = string.Empty;
    }

    public class ListingParametersDto
    {
        public int ParameterTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }
}

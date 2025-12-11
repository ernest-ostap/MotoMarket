namespace MotoMarket.Api.Controllers
{
    public class CreateListingApiRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public int ModelId { get; set; }
        public int VehicleCategoryId { get; set; }
        public int FuelTypeId { get; set; }
        public int GearboxTypeId { get; set; }
        public int DriveTypeId { get; set; }
        public int BodyTypeId { get; set; }
        public string VIN { get; set; }
        public int ProductionYear { get; set; }
        public int Mileage { get; set; }
        public string LocationCity { get; set; }
        public string LocationRegion { get; set; }
        public IFormFileCollection? Photos { get; set; }
    }
}

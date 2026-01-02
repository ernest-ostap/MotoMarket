using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Listings.Commands.BanListing;
using MotoMarket.Application.Listings.Commands.CreateListing;
using MotoMarket.Application.Listings.Commands.DeleteListing;
using MotoMarket.Application.Listings.Commands.RestoreListing;
using MotoMarket.Application.Listings.Commands.UnbanListing;
using MotoMarket.Application.Listings.Commands.UpdateListing;
using MotoMarket.Application.Listings.Commands.UpdateListingStatus;
using MotoMarket.Application.Listings.Queries.GetAdminListings;
using MotoMarket.Application.Listings.Queries.GetAllListings;
using MotoMarket.Application.Listings.Queries.GetListingDetail;
using MotoMarket.Application.Listings.Queries.GetMyListings;
using System.IO;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _environment;

        public ListingsController(IMediator mediator, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListingDto>>> GetAll([FromQuery] GetAllListingsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        //get dla admina - wszystkie ogłoszenia, także zbanowane i z innym statusem
        [HttpGet("admin-all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AdminListingDto>>> GetForAdmin()
        {
            return Ok(await _mediator.Send(new GetAdminListingsQuery()));
        }

        // GET api/listings/id
        [HttpGet("{id}")]
        public async Task<ActionResult<ListingDetailDto>> Get(int id)
        {
            var result = await _mediator.Send(new GetListingDetailQuery(id));

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //POST api/listings
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromForm] CreateListingApiRequest request) 
        {
            // 1. Logika zapisu zdjęć
            var photoUrls = new List<string>();
            var photoInputs = new List<ListingPhotoInput>();

            if (request.Photos != null && request.Photos.Count > 0)
            {
                // Ścieżka do folderu: wwwroot/uploads/listings
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "listings");

                // Upewnij się, że folder istnieje
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                for (int i = 0; i < request.Photos.Count; i++)
                {
                    var file = request.Photos[i];
                    // Generujemy unikalną nazwę pliku (żeby nie nadpisać innego)
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Fizyczny zapis na dysk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Dodajemy URL dostępny publicznie (np. https://localhost:7072/uploads/listings/xyz.jpg)
                    // W bazie trzymamy ścieżkę względną
                    var url = $"/uploads/listings/{uniqueFileName}";
                    photoUrls.Add(url);

                    var sortOrder = request.PhotoSortOrders != null && i < request.PhotoSortOrders.Count
                        ? request.PhotoSortOrders[i]
                        : i;

                    var isMain = request.MainPhotoIndex == i;

                    photoInputs.Add(new ListingPhotoInput
                    {
                        Url = url,
                        SortOrder = sortOrder,
                        IsMain = isMain
                    });
                }
            }

            // 2. Tworzymy właściwą Komendę dla Mediatora
            var command = new CreateListingCommand
            {
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                BrandId = request.BrandId,
                ModelId = request.ModelId,
                VehicleCategoryId = request.VehicleCategoryId,
                FuelTypeId = request.FuelTypeId,
                GearboxTypeId = request.GearboxTypeId,
                DriveTypeId = request.DriveTypeId,
                BodyTypeId = request.BodyTypeId,
                VIN = request.VIN,
                ProductionYear = request.ProductionYear,
                Mileage = request.Mileage,
                LocationCity = request.LocationCity,
                LocationRegion = request.LocationRegion,

                // Przekazujemy listę wygenerowanych URL-i
                PhotoUrls = photoUrls,
                Photos = photoInputs,

                Parameters = request.Parameters,
                SelectedFeatureIds = request.SelectedFeatureIds
            };
            var id = await _mediator.Send(command);

            //zwracamy z id nowo utworzonego ogłoszenia
            return Ok(id);
        }

        // PUT api/listings/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] UpdateListingApiRequest request)
        {
            // Security check: Czy ID w URL zgadza się z ID w ciele obiektu?
            if (id != request.Id)
            {
                return BadRequest();
            }

            var photoUrls = new List<string>();
            var photoInputs = new List<ListingPhotoInput>();

            if (request.Photos != null && request.Photos.Count > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "listings");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                for (int i = 0; i < request.Photos.Count; i++)
                {
                    var file = request.Photos[i];
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var url = $"/uploads/listings/{uniqueFileName}";
                    photoUrls.Add(url);

                    var sortOrder = request.PhotoSortOrders != null && i < request.PhotoSortOrders.Count
                        ? request.PhotoSortOrders[i]
                        : i;

                    var isMain = request.MainPhotoIndex == i;

                    photoInputs.Add(new ListingPhotoInput
                    {
                        Url = url,
                        SortOrder = sortOrder,
                        IsMain = isMain
                    });
                }
            }

            var command = new UpdateListingCommand
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                BrandId = request.BrandId,
                ModelId = request.ModelId,
                VehicleCategoryId = request.VehicleCategoryId,
                FuelTypeId = request.FuelTypeId,
                GearboxTypeId = request.GearboxTypeId,
                DriveTypeId = request.DriveTypeId,
                BodyTypeId = request.BodyTypeId,
                VIN = request.VIN,
                ProductionYear = request.ProductionYear,
                Mileage = request.Mileage,
                LocationCity = request.LocationCity,
                LocationRegion = request.LocationRegion,
                Photos = photoInputs,
                PhotoUrls = photoUrls,
                SelectedFeatureIds = request.SelectedFeatureIds ?? new List<int>(),
                Parameters = request.Parameters ?? new Dictionary<int, string>()
            };

            await _mediator.Send(command);

            return NoContent(); // Standardowy kod 204 dla Update
        }

        // DELETE api/listings
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteListingCommand(id));
            return NoContent(); // 204 No Content 
        }

        [Authorize]
        [HttpPost("{id}/restore")]
        public async Task<ActionResult> Restore(int id)
        {
            await _mediator.Send(new RestoreListingCommand(id));
            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/status")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateListingStatusCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("admin/{id}/ban")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BanListing(int id, [FromBody] string reason)
        {
            await _mediator.Send(new BanListingCommand(id, reason));
            return NoContent();
        }

        [HttpPatch("admin/{id}/unban")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnbanListing(int id)
        {
            await _mediator.Send(new UnbanListingCommand(id));
            return NoContent();
        }

        [Authorize] // Tylko dla zalogowanych!
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<ListingDto>>> GetMyListings()
        {
            return Ok(await _mediator.Send(new GetMyListingsQuery()));
        }
    }
}

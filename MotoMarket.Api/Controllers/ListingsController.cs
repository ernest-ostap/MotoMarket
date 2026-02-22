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

        // GET [admin] all listings (including banned / any status)
        [HttpGet("admin-all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AdminListingDto>>> GetForAdmin()
        {
            return Ok(await _mediator.Send(new GetAdminListingsQuery()));
        }

        // GET [id]
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

        // POST
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromForm] CreateListingApiRequest request) 
        {
            // 1. Save photos to disk
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

            // 2. Build command and send to mediator
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

                PhotoUrls = photoUrls,
                Photos = photoInputs,

                Parameters = request.Parameters,
                SelectedFeatureIds = request.SelectedFeatureIds
            };
            var id = await _mediator.Send(command);

            return Ok(id);
        }

        // PUT [id]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] UpdateListingApiRequest request)
        {
            if (id != request.Id)
                return BadRequest();

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

            return NoContent();
        }

        // DELETE [id]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteListingCommand(id));
            return NoContent();
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

        [Authorize]
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<ListingDto>>> GetMyListings()
        {
            return Ok(await _mediator.Send(new GetMyListingsQuery()));
        }
    }
}

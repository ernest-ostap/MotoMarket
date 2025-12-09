using MediatR;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Dictionaries.Queries;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionariesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DictionariesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetBrands()
        {
            return Ok(await _mediator.Send(new GetBrandsQuery()));
        }

        [HttpGet("models/{brandId}")]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetModels(int brandId)
        {
            return Ok(await _mediator.Send(new GetModelsByBrandQuery(brandId)));
        }

        [HttpGet("body-types")]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetBodyTypes()
        {
            return Ok(await _mediator.Send(new GetBodyTypesQuery()));
        }

        [HttpGet("drive-types")]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetDriveTypes()
        {
            return Ok(await _mediator.Send(new GetDriveTypesQuery()));
        }

        [HttpGet("fuel-types")]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetFuelTypes()
        {
            return Ok(await _mediator.Send(new GetFuelTypesQuery()));
        }

        [HttpGet("gearbox-types")]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetGearboxTypes()
        {
            return Ok(await _mediator.Send(new GetGearboxTypesQuery()));
        }

        [HttpGet("vehicle-categories")]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetVehicleCategories()
        {
            return Ok(await _mediator.Send(new GetVehicleCategoriesQuery()));
        }
    }
}
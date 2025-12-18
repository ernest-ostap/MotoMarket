using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Dictionaries.Commands.VehicleCategories;
using MotoMarket.Application.Dictionaries.Queries;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleCategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VehicleCategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetAll()
        {
            // true = CHCEMY też nieaktywne (dla Admina)
            return Ok(await _mediator.Send(new GetVehicleCategoriesQuery(true)));
        }

        // POST: api/VehicleCategories ()
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Create([FromBody] CreateVehicleCategoryCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVehicleCategoryCommand command)
        {
            if (id != command.Id) return BadRequest();
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{id}/toggle-active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _mediator.Send(new ToggleVehicleCategoryActiveCommand(id));
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteVehicleCategoryCommand(id));
            return NoContent();
        }
    }
}


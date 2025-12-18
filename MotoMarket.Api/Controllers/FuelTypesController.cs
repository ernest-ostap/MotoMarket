using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Dictionaries.Commands.FuelTypes;
using MotoMarket.Application.Dictionaries.Queries;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FuelTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetAll()
        {
            // true = CHCEMY też nieaktywne (dla Admina)
            return Ok(await _mediator.Send(new GetFuelTypesQuery(true)));
        }

        // POST: api/FuelTypes ()
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Create([FromBody] CreateFuelTypeCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFuelTypeCommand command)
        {
            if (id != command.Id) return BadRequest();
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{id}/toggle-active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _mediator.Send(new ToggleFuelTypeActiveCommand(id));
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteFuelTypeCommand(id));
            return NoContent();
        }
    }
}


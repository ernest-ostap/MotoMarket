using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Dictionaries.Commands.GearboxTypes;
using MotoMarket.Application.Dictionaries.Queries;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GearboxTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GearboxTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetAll()
        {
            // true = CHCEMY też nieaktywne (dla Admina)
            return Ok(await _mediator.Send(new GetGearboxTypesQuery(true)));
        }

        // POST: api/GearboxTypes ()
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Create([FromBody] CreateGearboxTypeCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGearboxTypeCommand command)
        {
            if (id != command.Id) return BadRequest();
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{id}/toggle-active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _mediator.Send(new ToggleGearboxTypeActiveCommand(id));
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteGearboxTypeCommand(id));
            return NoContent();
        }
    }
}


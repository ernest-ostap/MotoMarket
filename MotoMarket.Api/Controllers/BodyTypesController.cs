using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Dictionaries.Commands.BodyTypes;
using MotoMarket.Application.Dictionaries.Queries;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodyTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BodyTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetAll()
        {
            // true = CHCEMY też nieaktywne (dla Admina)
            return Ok(await _mediator.Send(new GetBodyTypesQuery(true)));
        }

        // POST: api/BodyTypes ()
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Create([FromBody] CreateBodyTypeCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBodyTypeCommand command)
        {
            if (id != command.Id) return BadRequest();
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{id}/toggle-active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _mediator.Send(new ToggleBodyTypeActiveCommand(id));
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteBodyTypeCommand(id));
            return NoContent();
        }
    }
}


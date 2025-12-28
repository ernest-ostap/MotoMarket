using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Dictionaries.Commands.ParameterTypes;
using MotoMarket.Application.Dictionaries.Queries;
using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParameterTypesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ParameterTypesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParameterTypeDto>>> GetAll()
            => Ok(await _mediator.Send(new GetAllParameterTypesQuery(true)));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Create([FromBody] CreateParameterTypeCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateParameterTypeCommand command)
        {
            if (id != command.Id) return BadRequest();
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{id}/toggle-active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _mediator.Send(new ToggleParameterTypeActiveCommand(id));
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteParameterTypeCommand(id));
            return NoContent();
        }
    }
}
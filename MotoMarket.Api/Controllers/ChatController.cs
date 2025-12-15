using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Chat.Queries;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Chat/history/USER_ID
        [HttpGet("history/{otherUserId}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetHistory(string otherUserId)
        {
            return Ok(await _mediator.Send(new GetConversationQuery(otherUserId)));
        }
    }
}
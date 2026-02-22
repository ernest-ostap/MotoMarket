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

        // GET history [otherUserId]
        [HttpGet("history/{otherUserId}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetHistory(string otherUserId)
        {
            return Ok(await _mediator.Send(new GetConversationQuery(otherUserId)));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConversationDto>>> GetMyConversations()
        {
            return Ok(await _mediator.Send(new GetMyConversationsQuery()));
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            return Ok(await _mediator.Send(new GetUnreadMessagesCountQuery()));
        }
    }
}
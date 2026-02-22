using MediatR; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MotoMarket.Application.Chat.Commands;
using MotoMarket.Application.Common.Interfaces.Identity;
using System.Security.Claims;

namespace MotoMarket.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IUserManagementService _userManagementService;

        public ChatHub(IMediator mediator, IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
            _mediator = mediator;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            if (await _userManagementService.IsUserBanned(userId))
            {
                Context.Abort(); // Odrzuć połączenie
                return;
            }

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string recipientId, string message, int? listingId)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId)) return;
            if (await _userManagementService.IsUserBanned(senderId))
            {
                
                Context.Abort();
                return;
            }

            // 1. Persist message (command)
            var command = new SendMessageCommand
            {
                RecipientId = recipientId,
                Content = message,
                ListingId = listingId
            };
            await _mediator.Send(command);

            // 2. Broadcast to recipient
            await Clients.User(recipientId).SendAsync("ReceiveMessage", senderId, message, listingId);

            
        }
    }
}
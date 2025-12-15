using MediatR; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MotoMarket.Application.Chat.Commands; 
using System.Security.Claims;

namespace MotoMarket.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator; 

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendMessage(string recipientId, string message, int? listingId)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId)) return;

            // 1. ZAPIS DO BAZY (przez Command)
            var command = new SendMessageCommand
            {
                RecipientId = recipientId,
                Content = message,
                ListingId = listingId
            };

            // Czekamy aż się zapisze, żeby mieć pewność
            await _mediator.Send(command);

            // 2. WYSYŁKA REAL-TIME
            // Wysyłamy do Odbiorcy
            await Clients.User(recipientId).SendAsync("ReceiveMessage", senderId, message, listingId);

            
        }
    }
}
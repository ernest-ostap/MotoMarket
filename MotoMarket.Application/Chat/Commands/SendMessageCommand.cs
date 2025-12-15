using MediatR;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Chat.Commands
{
    public class SendMessageCommand : IRequest<int>
    {
        public string RecipientId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int? ListingId { get; set; }
    }

    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public SendMessageCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var senderId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(senderId)) throw new UnauthorizedAccessException();

            var entity = new ChatMessage
            {
                SenderId = senderId,
                RecipientId = request.RecipientId,
                Content = request.Content,
                ListingId = request.ListingId,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.ChatMessages.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}

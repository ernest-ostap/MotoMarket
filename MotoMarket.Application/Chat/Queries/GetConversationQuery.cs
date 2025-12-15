using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Chat.Queries
{
    // Pobiera rozmowę z konkretnym użytkownikiem (OtherUserId)
    public record GetConversationQuery(string OtherUserId) : IRequest<IEnumerable<ChatMessageDto>>;

    public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, IEnumerable<ChatMessageDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetConversationQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ChatMessageDto>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(currentUserId)) return new List<ChatMessageDto>();

            // Pobieramy wiadomości gdzie:
            // (Ja jestem nadawcą I On jest odbiorcą) LUB (On jest nadawcą I Ja jestem odbiorcą)
            var messages = await _context.ChatMessages
                .AsNoTracking()
                .Where(m => (m.SenderId == currentUserId && m.RecipientId == request.OtherUserId) ||
                            (m.SenderId == request.OtherUserId && m.RecipientId == currentUserId))
                .OrderBy(m => m.SentAt) // Najstarsze na górze
                .Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    IsMine = m.SenderId == currentUserId // Flagujemy "moje" wiadomości
                })
                .ToListAsync(cancellationToken);

            return messages;
        }
    }
}
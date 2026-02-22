using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Common.Interfaces.Persistence;

namespace MotoMarket.Application.Chat.Queries
{
    public record GetMyConversationsQuery : IRequest<IEnumerable<ConversationDto>>;

    public class GetMyConversationsQueryHandler : IRequestHandler<GetMyConversationsQuery, IEnumerable<ConversationDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetMyConversationsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ConversationDto>> Handle(GetMyConversationsQuery request, CancellationToken cancellationToken)
        {
            var myId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(myId)) return new List<ConversationDto>();

            // 1. Pobieramy wszystkie wiadomości, w których biorę udział
            var allMyMessages = await _context.ChatMessages
                .AsNoTracking()
                .Where(m => m.SenderId == myId || m.RecipientId == myId)
                .OrderByDescending(m => m.SentAt) // Najnowsze na górze
                .ToListAsync(cancellationToken);

            if (!allMyMessages.Any()) return new List<ConversationDto>();

            // 2. Grupujemy wiadomości po "Tym Drugim" użytkowniku
            var groupedConversations = allMyMessages
                .GroupBy(m => m.SenderId == myId ? m.RecipientId : m.SenderId)
                .ToList();

            // 3. Pobieramy listę ID rozmówców
            var contactIds = groupedConversations.Select(g => g.Key).Distinct().ToList();

            // 4. Load user data (display names, emails)
            var usersInfo = await _context.Users
                .AsNoTracking()
                .Where(u => contactIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email
                })
                .ToDictionaryAsync(u => u.Id, cancellationToken);

            // 5. Sklejamy wynik (Wiadomości + Imiona)
            var result = groupedConversations.Select(g =>
            {
                var otherUserId = g.Key;
                var lastMsg = g.First(); // Najnowsza wiadomość w grupie

                // Próbujemy znaleźć usera w słowniku
                string displayName = "Użytkownik usunięty";
                if (usersInfo.TryGetValue(otherUserId, out var user))
                {
                    // Formatowanie: "Jan Kowalski" lub Email jak brak imienia
                    displayName = $"{user.FirstName} {user.LastName}".Trim();
                    if (string.IsNullOrEmpty(displayName))
                    {
                        displayName = user.Email ?? "Nieznany";
                    }
                }

                return new ConversationDto
                {
                    OtherUserId = otherUserId,
                    OtherUserName = displayName,
                    LastMessage = lastMsg.Content,
                    LastMessageDate = lastMsg.SentAt,
                    ListingId = lastMsg.ListingId
                };
            })
            .ToList();

            return result;
        }
    }
}
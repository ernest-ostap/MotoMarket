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

            // Pobieramy wszystkie wiadomości, w których biorę udział
            var allMyMessages = await _context.ChatMessages
                .AsNoTracking()
                .Where(m => m.SenderId == myId || m.RecipientId == myId)
                .OrderByDescending(m => m.SentAt) // Najnowsze na górze
                .ToListAsync(cancellationToken);

            // Grupujemy w pamięci (prostsze niż skomplikowany GroupBy w SQL przy GUID-ach)
            var conversations = allMyMessages
                .GroupBy(m => m.SenderId == myId ? m.RecipientId : m.SenderId) // Grupuj po ID rozmówcy
                .Select(g => new ConversationDto
                {
                    OtherUserId = g.Key,
                    LastMessage = g.First().Content, // Pierwsza, bo posortowaliśmy wcześniej
                    LastMessageDate = g.First().SentAt,
                    ListingId = g.First().ListingId
                })
                .ToList();

            return conversations;
        }
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Common.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Chat.Queries
{
    public record GetUnreadMessagesCountQuery : IRequest<int>;

    public class GetUnreadMessagesCountQueryHandler : IRequestHandler<GetUnreadMessagesCountQuery, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetUnreadMessagesCountQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(GetUnreadMessagesCountQuery request, CancellationToken cancellationToken)
        {
            var myId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(myId)) return 0;

            return await _context.ChatMessages
                .AsNoTracking()
                .CountAsync(m => m.RecipientId == myId && !m.IsRead, cancellationToken);
        }
    }
}

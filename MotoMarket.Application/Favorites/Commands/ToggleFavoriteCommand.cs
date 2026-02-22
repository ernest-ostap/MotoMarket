using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Domain.Entities;
using MotoMarket.Domain.Entities.Listings;

namespace MotoMarket.Application.Favorites.Commands
{
    /// <summary>Returns true if added, false if removed.</summary>
    public record ToggleFavoriteCommand(int ListingId) : IRequest<bool>;

    public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ToggleFavoriteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException();

            var existing = await _context.UserFavorites
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ListingId == request.ListingId, cancellationToken);

            if (existing != null)
            {
                _context.UserFavorites.Remove(existing);
                await _context.SaveChangesAsync(cancellationToken);
                return false;
            }

            var favorite = new UserFavorite { UserId = userId, ListingId = request.ListingId };
            _context.UserFavorites.Add(favorite);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
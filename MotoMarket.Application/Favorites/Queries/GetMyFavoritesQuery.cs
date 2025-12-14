using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoMarket.Application.Common.Interfaces;
using MotoMarket.Application.Common.Interfaces.Identity;
using MotoMarket.Application.Common.Interfaces.Persistence;
using MotoMarket.Application.Listings.Queries.GetAllListings; // Używamy ListingDto

namespace MotoMarket.Application.Favorites.Queries
{
    public record GetMyFavoritesQuery : IRequest<IEnumerable<ListingDto>>;

    public class GetMyFavoritesQueryHandler : IRequestHandler<GetMyFavoritesQuery, IEnumerable<ListingDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetMyFavoritesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ListingDto>> Handle(GetMyFavoritesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId)) return new List<ListingDto>();

            return await _context.UserFavorites
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.Listing).ThenInclude(l => l.Brand)
                .Include(x => x.Listing).ThenInclude(l => l.Model)
                .Include(x => x.Listing).ThenInclude(l => l.Photos)
                .Select(x => x.Listing) // Wybieramy samo ogłoszenie
                .Select(x => new ListingDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price,
                    BrandName = x.Brand.Name,
                    ModelName = x.Model.Name,
                    LocationCity = x.LocationCity,
                    ProductionYear = x.ProductionYear,
                    Mileage = x.Mileage,
                    MainPhotoUrl = x.Photos.FirstOrDefault(p => p.IsMain).Url ?? "",
                    Status = (int)x.Status,
                    CreatedAt = x.CreatedAt,
                    ExpiresAt = x.ExpiresAt,
                    IsFavorite = true // Wszystkie tutaj są ulubione
                })
                .ToListAsync(cancellationToken);
        }
    }
}
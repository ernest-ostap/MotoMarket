using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Favorites.Commands;
using MotoMarket.Application.Favorites.Queries;
using MotoMarket.Application.Listings.Queries.GetAllListings; // ListingDto

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Wszystko tutaj wymaga logowania
    public class FavoritesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FavoritesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{listingId}")]
        public async Task<ActionResult<bool>> Toggle(int listingId)
        {
            // Zwraca true (dodano) lub false (usunięto)
            return Ok(await _mediator.Send(new ToggleFavoriteCommand(listingId)));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListingDto>>> GetMyFavorites()
        {
            return Ok(await _mediator.Send(new GetMyFavoritesQuery()));
        }
    }
}
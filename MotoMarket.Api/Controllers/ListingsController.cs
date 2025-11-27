using Microsoft.AspNetCore.Mvc;
using MediatR;
using MotoMarket.Application.Listings.Commands.CreateListing;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ListingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //POST api/listings
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateListingCommand command)
        {
            //dzięki CQRS i MediatR nie musimy pisać logiki tworzenia ogłoszenia w kontrolerze
            //a wysyłamy polecenie do odpowiedniego handlera
            var id = await _mediator.Send(command);

            //zwracamy z id nowo utworzonego ogłoszenia
            return Ok(id);
        }
    }
}

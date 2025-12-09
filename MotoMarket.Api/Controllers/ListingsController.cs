using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Listings.Commands.CreateListing;
using MotoMarket.Application.Listings.Commands.DeleteListing;
using MotoMarket.Application.Listings.Commands.UpdateListing;
using MotoMarket.Application.Listings.Queries.GetAllListings;
using MotoMarket.Application.Listings.Queries.GetListingDetail;
using MotoMarket.Application.Listings.Queries.GetMyListings;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListingDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllListingsQuery());
            return Ok(result);
        }

        // GET api/listings/id
        [HttpGet("{id}")]
        public async Task<ActionResult<ListingDetailDto>> Get(int id)
        {
            var result = await _mediator.Send(new GetListingDetailQuery(id));

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
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

        // PUT api/listings/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateListingCommand command)
        {
            // Security check: Czy ID w URL zgadza się z ID w ciele obiektu?
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command);

            return NoContent(); // Standardowy kod 204 dla Update
        }

        // DELETE api/listings
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteListingCommand(id));
            return NoContent(); // 204 No Content 
        }

        [Authorize] // Tylko dla zalogowanych!
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<ListingDto>>> GetMyListings()
        {
            return Ok(await _mediator.Send(new GetMyListingsQuery()));
        }
    }
}

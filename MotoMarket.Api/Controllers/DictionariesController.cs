using MediatR;
using Microsoft.AspNetCore.Mvc;
using MotoMarket.Application.Dictionaries.Queries;

namespace MotoMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionariesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DictionariesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetBrands()
        {
            return Ok(await _mediator.Send(new GetBrandsQuery()));
        }

        [HttpGet("models/{brandId}")]
        public async Task<ActionResult<IEnumerable<DictionaryDto>>> GetModels(int brandId)
        {
            return Ok(await _mediator.Send(new GetModelsByBrandQuery(brandId)));
        }

        // ... Tutaj dopisz metody dla paliw, skrzyń itp., jak dorobisz handlery
        // [HttpGet("fuel-types")] ...

        
    }
}
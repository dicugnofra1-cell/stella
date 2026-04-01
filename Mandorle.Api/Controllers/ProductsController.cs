using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mandorle.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateProductCommand command)
        //{
        //    var result = await _mediator.Send(command);
        //    return Ok(result);
        //}
    }
}

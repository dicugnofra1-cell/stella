using Mandorle.Application.Products.Commands;
using Mandorle.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mandorle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] bool? active,
        [FromQuery] bool? channelB2BEnabled,
        [FromQuery] bool? channelB2CEnabled)
    {
        var result = await _mediator.Send(new SearchProductsQuery(search, category, active, channelB2BEnabled, channelB2CEnabled));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-sku")]
    public async Task<IActionResult> GetBySku([FromQuery] string sku)
    {
        var result = await _mediator.Send(new GetProductBySkuQuery(sku));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("exists")]
    public async Task<IActionResult> Exists([FromQuery] string sku, [FromQuery] int? excludeProductId)
    {
        var result = await _mediator.Send(new CheckProductExistenceQuery(sku, excludeProductId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:int}/active")]
    public async Task<IActionResult> SetActive(int id, [FromBody] SetProductActiveCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound();
    }
}

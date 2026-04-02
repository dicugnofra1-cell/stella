using Mandorle.Application.Inventory.Commands;
using Mandorle.Application.Inventory.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mandorle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryMovementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryMovementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] int? productId,
        [FromQuery] int? batchId,
        [FromQuery] string? movementType,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var result = await _mediator.Send(new SearchInventoryMovementsQuery(productId, batchId, movementType, fromDate, toDate));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetInventoryMovementByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("balance/by-batch/{batchId:int}")]
    public async Task<IActionResult> GetBalanceByBatch(int batchId)
    {
        var result = await _mediator.Send(new GetInventoryBalanceByBatchQuery(batchId));
        return Ok(result);
    }

    [HttpGet("balance/by-product/{productId:int}")]
    public async Task<IActionResult> GetBalanceByProduct(int productId)
    {
        var result = await _mediator.Send(new GetInventoryBalanceByProductQuery(productId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInventoryMovementCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}

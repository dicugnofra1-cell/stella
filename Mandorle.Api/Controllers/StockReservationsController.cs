using Mandorle.Application.StockReservations.Commands;
using Mandorle.Application.StockReservations.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mandorle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] int? orderId,
        [FromQuery] int? orderItemId,
        [FromQuery] int? productId,
        [FromQuery] int? batchId,
        [FromQuery] string? status,
        [FromQuery] string? reservationType)
    {
        var result = await _mediator.Send(new SearchStockReservationsQuery(orderId, orderItemId, productId, batchId, status, reservationType));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetStockReservationByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStockReservationCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> SetStatus(int id, [FromBody] SetStockReservationStatusCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound();
    }
}

using Mandorle.Application.Quality.Commands;
using Mandorle.Application.Quality.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mandorle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QualityChecksController : ControllerBase
{
    private readonly IMediator _mediator;

    public QualityChecksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetQualityCheckByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-batch/{batchId:int}")]
    public async Task<IActionResult> GetByBatch(int batchId)
    {
        var result = await _mediator.Send(new GetQualityChecksByBatchQuery(batchId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQualityCheckCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQualityCheckCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }
}

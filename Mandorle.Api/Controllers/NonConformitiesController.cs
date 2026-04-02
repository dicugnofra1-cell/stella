using Mandorle.Application.Quality.Commands;
using Mandorle.Application.Quality.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mandorle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NonConformitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NonConformitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] int? batchId, [FromQuery] string? severity, [FromQuery] string? status)
    {
        var result = await _mediator.Send(new SearchNonConformitiesQuery(batchId, severity, status));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetNonConformityByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNonConformityCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNonConformityCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:int}/close")]
    public async Task<IActionResult> Close(int id, [FromBody] CloseNonConformityCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound();
    }
}

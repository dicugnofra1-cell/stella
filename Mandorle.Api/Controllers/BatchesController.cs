using Mandorle.Application.Batches.Commands;
using Mandorle.Application.Batches.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mandorle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatchesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BatchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? search,
        [FromQuery] int? productId,
        [FromQuery] int? supplierId,
        [FromQuery] string? batchType,
        [FromQuery] string? status,
        [FromQuery] bool? bioFlag)
    {
        var result = await _mediator.Send(new SearchBatchesQuery(search, productId, supplierId, batchType, status, bioFlag));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetBatchByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-code")]
    public async Task<IActionResult> GetByCode([FromQuery] string batchCode)
    {
        var result = await _mediator.Send(new GetBatchByCodeQuery(batchCode));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("exists")]
    public async Task<IActionResult> Exists([FromQuery] string batchCode, [FromQuery] int? excludeBatchId)
    {
        var result = await _mediator.Send(new CheckBatchExistenceQuery(batchCode, excludeBatchId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBatchCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBatchCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> SetStatus(int id, [FromBody] SetBatchStatusCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("{childBatchId:int}/parents")]
    public async Task<IActionResult> GetParents(int childBatchId)
    {
        var result = await _mediator.Send(new GetBatchParentsQuery(childBatchId));
        return Ok(result);
    }

    [HttpGet("{parentBatchId:int}/children")]
    public async Task<IActionResult> GetChildren(int parentBatchId)
    {
        var result = await _mediator.Send(new GetBatchChildrenQuery(parentBatchId));
        return Ok(result);
    }

    [HttpGet("{childBatchId:int}/parents/{batchLinkId:int}")]
    public async Task<IActionResult> GetBatchLinkById(int childBatchId, int batchLinkId)
    {
        var result = await _mediator.Send(new GetBatchLinkByIdQuery(childBatchId, batchLinkId));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("links")]
    public async Task<IActionResult> AddBatchLink([FromBody] AddBatchLinkCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBatchLinkById), new { childBatchId = result.ChildBatchId, batchLinkId = result.Id }, result);
    }

    [HttpPut("{childBatchId:int}/parents/{batchLinkId:int}")]
    public async Task<IActionResult> UpdateBatchLink(int childBatchId, int batchLinkId, [FromBody] UpdateBatchLinkCommand command)
    {
        if (childBatchId != command.ChildBatchId || batchLinkId != command.BatchLinkId)
        {
            return BadRequest("Route ids and payload ids must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }
}

using Mandorle.Application.Traceability.Commands;
using Mandorle.Application.Traceability.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mandorle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TraceController : ControllerBase
{
    private readonly IMediator _mediator;

    public TraceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{batchCode}")]
    public async Task<IActionResult> GetByBatchCode(string batchCode)
    {
        var result = await _mediator.Send(new GetPublicTraceViewByBatchCodeQuery(batchCode));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-batch/{batchId:int}")]
    public async Task<IActionResult> GetByBatchId(int batchId)
    {
        var result = await _mediator.Send(new GetPublicTraceViewByBatchIdQuery(batchId));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertPublicTraceViewCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

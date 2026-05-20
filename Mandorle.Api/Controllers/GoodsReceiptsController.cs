using Mandorle.Application.GoodsReceipts.Commands;
using Mandorle.Application.GoodsReceipts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mandorle.Api.Controllers;

[ApiController]
[Route("api/goods-receipts")]
public class GoodsReceiptsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GoodsReceiptsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetToday([FromQuery] string? search)
    {
        var result = await _mediator.Send(new GetTodayGoodsReceiptsQuery(search), HttpContext.RequestAborted);
        return Ok(result);
    }

    [HttpGet("active-history")]
    public async Task<IActionResult> GetActiveHistory([FromQuery] string? search)
    {
        var result = await _mediator.Send(new GetActiveGoodsReceiptsHistoryQuery(search), HttpContext.RequestAborted);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterGoodsReceiptCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{batchId:int}")]
    public async Task<IActionResult> Update(int batchId, [FromBody] UpdateGoodsReceiptCommand command)
    {
        if (batchId != command.BatchId)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

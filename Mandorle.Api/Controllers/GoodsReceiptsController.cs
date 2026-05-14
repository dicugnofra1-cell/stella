using Mandorle.Application.GoodsReceipts.Commands;
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

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterGoodsReceiptCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

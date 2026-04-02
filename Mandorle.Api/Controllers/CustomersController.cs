using Mandorle.Application.Customers.Commands;
using Mandorle.Application.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mandorle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] string? type, [FromQuery] string? status)
    {
        var result = await _mediator.Send(new SearchCustomersQuery(search, type, status));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] bool includeAddresses = true)
    {
        var result = await _mediator.Send(new GetCustomerByIdQuery(id, includeAddresses));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-email")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var result = await _mediator.Send(new GetCustomerByEmailQuery(email));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-pec")]
    public async Task<IActionResult> GetByPec([FromQuery] string pec)
    {
        var result = await _mediator.Send(new GetCustomerByPecQuery(pec));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-spid")]
    public async Task<IActionResult> GetBySpid([FromQuery] string spidIdentifier)
    {
        var result = await _mediator.Send(new GetCustomerBySpidIdentifierQuery(spidIdentifier));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-vat-number")]
    public async Task<IActionResult> GetByVatNumber([FromQuery] string vatNumber)
    {
        var result = await _mediator.Send(new GetCustomerByVatNumberQuery(vatNumber));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("exists")]
    public async Task<IActionResult> Exists(
        [FromQuery] string? email,
        [FromQuery] string? pec,
        [FromQuery] string? spidIdentifier,
        [FromQuery] string? vatNumber,
        [FromQuery] int? excludeCustomerId)
    {
        var result = await _mediator.Send(new CheckCustomerExistenceQuery(email, pec, spidIdentifier, vatNumber, excludeCustomerId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> SetStatus(int id, [FromBody] SetCustomerStatusCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("{customerId:int}/addresses")]
    public async Task<IActionResult> GetAddresses(int customerId)
    {
        var result = await _mediator.Send(new GetCustomerAddressesQuery(customerId));
        return Ok(result);
    }

    [HttpGet("{customerId:int}/addresses/default")]
    public async Task<IActionResult> GetDefaultAddress(int customerId, [FromQuery] string addressType)
    {
        var result = await _mediator.Send(new GetDefaultCustomerAddressByTypeQuery(customerId, addressType));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{customerId:int}/addresses")]
    public async Task<IActionResult> AddAddress(int customerId, [FromBody] AddCustomerAddressCommand command)
    {
        if (customerId != command.CustomerId)
        {
            return BadRequest("Route customerId and payload customerId must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : CreatedAtAction(nameof(GetDefaultAddress), new { customerId, addressType = result.AddressType }, result);
    }

    [HttpPut("{customerId:int}/addresses/{addressId:int}")]
    public async Task<IActionResult> UpdateAddress(int customerId, int addressId, [FromBody] UpdateCustomerAddressCommand command)
    {
        if (customerId != command.CustomerId || addressId != command.AddressId)
        {
            return BadRequest("Route ids and payload ids must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{customerId:int}/addresses/{addressId:int}/default")]
    public async Task<IActionResult> SetDefaultAddress(int customerId, int addressId)
    {
        var result = await _mediator.Send(new SetDefaultCustomerAddressCommand(customerId, addressId));
        return result ? NoContent() : NotFound();
    }
}

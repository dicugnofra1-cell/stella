using Mandorle.Application.Suppliers.Commands;
using Mandorle.Application.Suppliers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mandorle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] string? status)
    {
        var result = await _mediator.Send(new SearchSuppliersQuery(search, status));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetSupplierByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-email")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var result = await _mediator.Send(new GetSupplierByEmailQuery(email));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-vat-number")]
    public async Task<IActionResult> GetByVatNumber([FromQuery] string vatNumber)
    {
        var result = await _mediator.Send(new GetSupplierByVatNumberQuery(vatNumber));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("exists")]
    public async Task<IActionResult> Exists([FromQuery] string? email, [FromQuery] string? vatNumber, [FromQuery] int? excludeSupplierId)
    {
        var result = await _mediator.Send(new CheckSupplierExistenceQuery(email, vatNumber, excludeSupplierId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> SetStatus(int id, [FromBody] SetSupplierStatusCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("{supplierId:int}/documents")]
    public async Task<IActionResult> GetDocuments(int supplierId)
    {
        var result = await _mediator.Send(new GetSupplierDocumentsQuery(supplierId));
        return Ok(result);
    }

    [HttpGet("{supplierId:int}/documents/{documentId:int}")]
    public async Task<IActionResult> GetDocumentById(int supplierId, int documentId)
    {
        var result = await _mediator.Send(new GetSupplierDocumentByIdQuery(supplierId, documentId));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{supplierId:int}/documents")]
    public async Task<IActionResult> AddDocument(int supplierId, [FromBody] AddSupplierDocumentCommand command)
    {
        if (supplierId != command.SupplierId)
        {
            return BadRequest("Route supplierId and payload supplierId must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : CreatedAtAction(nameof(GetDocumentById), new { supplierId, documentId = result.Id }, result);
    }

    [HttpPut("{supplierId:int}/documents/{documentId:int}")]
    public async Task<IActionResult> UpdateDocument(int supplierId, int documentId, [FromBody] UpdateSupplierDocumentCommand command)
    {
        if (supplierId != command.SupplierId || documentId != command.DocumentId)
        {
            return BadRequest("Route ids and payload ids must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{supplierId:int}/certifications")]
    public async Task<IActionResult> GetCertifications(int supplierId, [FromQuery] string? type, [FromQuery] string? status)
    {
        var result = await _mediator.Send(new GetSupplierCertificationsQuery(supplierId, type, status));
        return Ok(result);
    }

    [HttpGet("{supplierId:int}/certifications/{certificationId:int}")]
    public async Task<IActionResult> GetCertificationById(int supplierId, int certificationId)
    {
        var result = await _mediator.Send(new GetCertificationByIdQuery(supplierId, certificationId));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{supplierId:int}/certifications")]
    public async Task<IActionResult> AddCertification(int supplierId, [FromBody] AddCertificationCommand command)
    {
        if (supplierId != command.SupplierId)
        {
            return BadRequest("Route supplierId and payload supplierId must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : CreatedAtAction(nameof(GetCertificationById), new { supplierId, certificationId = result.Id }, result);
    }

    [HttpPut("{supplierId:int}/certifications/{certificationId:int}")]
    public async Task<IActionResult> UpdateCertification(int supplierId, int certificationId, [FromBody] UpdateCertificationCommand command)
    {
        if (supplierId != command.SupplierId || certificationId != command.CertificationId)
        {
            return BadRequest("Route ids and payload ids must match.");
        }

        var result = await _mediator.Send(command);
        return result is null ? NotFound() : Ok(result);
    }
}

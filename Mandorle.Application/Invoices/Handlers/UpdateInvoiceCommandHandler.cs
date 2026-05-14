using Mandorle.Application.Invoices.Commands;
using Mandorle.Application.Invoices.Mapping;
using Mandorle.Application.Invoices.Models;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Invoices.Handlers;

public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, InvoiceDto?>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public UpdateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<InvoiceDto?> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invoice is null)
        {
            return null;
        }

        if (await _invoiceRepository.ExistsByDocumentNumberAsync(request.DocumentNumber, request.Id, cancellationToken))
        {
            throw new InvalidOperationException("Esiste gia una fattura con lo stesso numero documento.");
        }

        invoice.DocumentNumber = request.DocumentNumber;
        invoice.DocumentType = request.DocumentType;
        invoice.IssueDate = request.IssueDate;
        invoice.Notes = request.Notes;
        invoice.UpdatedAt = DateTime.UtcNow;

        _invoiceRepository.Update(invoice);
        await _invoiceRepository.SaveChangesAsync(cancellationToken);

        return invoice.ToDto();
    }
}

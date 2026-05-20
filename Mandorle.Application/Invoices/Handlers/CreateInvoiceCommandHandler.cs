using Mandorle.Application.Invoices.Commands;
using Mandorle.Application.Invoices.Mapping;
using Mandorle.Application.Invoices.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Enums;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Invoices.Handlers;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;

    public CreateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository)
    {
        _invoiceRepository = invoiceRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }

    public async Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, includeItems: true, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("L'ordine selezionato non esiste.");
        var customer = await _customerRepository.GetByIdAsync(order.CustomerId, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Il cliente collegato all'ordine non esiste.");

        if (!order.OrderType.Equals("B2B", StringComparison.OrdinalIgnoreCase) || !customer.Type.Equals("B2B", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("La fattura V1 e consentita solo per clienti B2B.");
        }

        if (string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            throw new InvalidOperationException("Il numero documento e obbligatorio.");
        }

        if (!OperationalEnumMappings.TryParseInvoiceDocumentType(request.DocumentType, out var documentType))
        {
            throw new InvalidOperationException("Il tipo documento deve essere FATTURA o DDT.");
        }

        if (string.IsNullOrWhiteSpace(customer.VatNumber))
        {
            throw new InvalidOperationException("Il cliente B2B deve avere una partita IVA valorizzata.");
        }

        if (await _invoiceRepository.GetByOrderIdAsync(order.Id, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Per questo ordine esiste gia un documento associato.");
        }

        if (await _invoiceRepository.ExistsByDocumentNumberAsync(request.DocumentNumber, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("Esiste gia una fattura con lo stesso numero documento.");
        }

        var invoice = new Invoice
        {
            OrderId = order.Id,
            CustomerId = customer.Id,
            DocumentNumber = request.DocumentNumber,
            DocumentType = documentType.ToDbValue(),
            Source = "STELLA",
            SyncStatus = "PREPARATO",
            TotalAmount = order.TotalAmount,
            Currency = order.Currency,
            IssueDate = request.IssueDate ?? DateTime.UtcNow,
            ExternalProvider = null,
            ExternalDocumentId = null,
            ExternalDocumentNumber = null,
            ExternalSyncAt = null,
            ExternalSyncError = null,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _invoiceRepository.SaveChangesAsync(cancellationToken);

        return invoice.ToDto();
    }
}

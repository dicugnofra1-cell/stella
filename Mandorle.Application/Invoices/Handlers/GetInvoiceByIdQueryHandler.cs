using Mandorle.Application.Invoices.Mapping;
using Mandorle.Application.Invoices.Models;
using Mandorle.Application.Invoices.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Invoices.Handlers;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id, cancellationToken);
        return invoice?.ToDto();
    }
}

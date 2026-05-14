using Mandorle.Application.Invoices.Mapping;
using Mandorle.Application.Invoices.Models;
using Mandorle.Application.Invoices.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Invoices.Handlers;

public class SearchInvoicesQueryHandler : IRequestHandler<SearchInvoicesQuery, IReadOnlyList<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public SearchInvoicesQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<IReadOnlyList<InvoiceDto>> Handle(SearchInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.SearchAsync(
            request.OrderId,
            request.CustomerId,
            request.DocumentType,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        return invoices.Select(invoice => invoice.ToDto()).ToList();
    }
}

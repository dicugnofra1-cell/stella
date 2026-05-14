using Mandorle.Application.Invoices.Models;
using MediatR;

namespace Mandorle.Application.Invoices.Queries;

public record SearchInvoicesQuery(
    int? OrderId,
    int? CustomerId,
    string? DocumentType,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<IReadOnlyList<InvoiceDto>>;

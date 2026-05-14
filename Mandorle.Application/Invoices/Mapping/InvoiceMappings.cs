using Mandorle.Application.Invoices.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Invoices.Mapping;

internal static class InvoiceMappings
{
    public static InvoiceDto ToDto(this Invoice invoice)
    {
        return new InvoiceDto(
            invoice.Id,
            invoice.OrderId,
            invoice.CustomerId,
            invoice.DocumentNumber,
            invoice.DocumentType,
            invoice.TotalAmount,
            invoice.Currency,
            invoice.IssueDate,
            invoice.Notes);
    }
}

using Mandorle.Application.Invoices.Models;
using MediatR;

namespace Mandorle.Application.Invoices.Commands;

public record CreateInvoiceCommand(
    int OrderId,
    string DocumentNumber,
    string DocumentType,
    DateTime? IssueDate,
    string? Notes) : IRequest<InvoiceDto>;

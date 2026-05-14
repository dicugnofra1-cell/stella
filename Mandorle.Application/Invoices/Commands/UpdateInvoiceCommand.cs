using Mandorle.Application.Invoices.Models;
using MediatR;

namespace Mandorle.Application.Invoices.Commands;

public record UpdateInvoiceCommand(
    int Id,
    string DocumentNumber,
    string DocumentType,
    DateTime IssueDate,
    string? Notes) : IRequest<InvoiceDto?>;

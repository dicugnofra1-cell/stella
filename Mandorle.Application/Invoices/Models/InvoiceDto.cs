namespace Mandorle.Application.Invoices.Models;

public record InvoiceDto(
    int Id,
    int OrderId,
    int CustomerId,
    string DocumentNumber,
    string DocumentType,
    decimal TotalAmount,
    string Currency,
    DateTime IssueDate,
    string? Notes);

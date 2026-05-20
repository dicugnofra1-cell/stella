namespace Mandorle.Application.Invoices.Models;

public record InvoiceDto(
    int Id,
    int OrderId,
    int CustomerId,
    string DocumentNumber,
    string DocumentType,
    string Source,
    string SyncStatus,
    decimal TotalAmount,
    string Currency,
    DateTime IssueDate,
    string? ExternalProvider,
    string? ExternalDocumentId,
    string? ExternalDocumentNumber,
    DateTime? ExternalSyncAt,
    string? ExternalSyncError,
    string? Notes);

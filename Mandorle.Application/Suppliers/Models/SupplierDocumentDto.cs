namespace Mandorle.Application.Suppliers.Models;

public record SupplierDocumentDto(
    int Id,
    int SupplierId,
    string DocumentType,
    string FileName,
    string StoragePath,
    DateTime UploadedAt,
    string? UploadedBy);

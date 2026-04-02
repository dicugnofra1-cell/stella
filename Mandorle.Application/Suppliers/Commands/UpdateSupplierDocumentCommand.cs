using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Commands;

public record UpdateSupplierDocumentCommand(
    int SupplierId,
    int DocumentId,
    string DocumentType,
    string FileName,
    string StoragePath,
    string? UploadedBy) : IRequest<SupplierDocumentDto?>;

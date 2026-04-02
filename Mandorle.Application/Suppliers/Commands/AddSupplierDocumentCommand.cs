using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Commands;

public record AddSupplierDocumentCommand(
    int SupplierId,
    string DocumentType,
    string FileName,
    string StoragePath,
    string? UploadedBy) : IRequest<SupplierDocumentDto?>;

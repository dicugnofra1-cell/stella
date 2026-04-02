using Mandorle.Application.Suppliers.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Suppliers.Mapping;

internal static class SupplierMappings
{
    public static SupplierDto ToDto(this Supplier supplier)
    {
        return new SupplierDto(
            supplier.Id,
            supplier.Name,
            supplier.VatNumber,
            supplier.Address,
            supplier.Email,
            supplier.Phone,
            supplier.Status);
    }

    public static SupplierDocumentDto ToDto(this SupplierDocument document)
    {
        return new SupplierDocumentDto(
            document.Id,
            document.SupplierId,
            document.DocumentType,
            document.FileName,
            document.StoragePath,
            document.UploadedAt,
            document.UploadedBy);
    }

    public static CertificationDto ToDto(this Certification certification)
    {
        return new CertificationDto(
            certification.Id,
            certification.SupplierId,
            certification.Type,
            certification.Authority,
            certification.Code,
            certification.IssueDate,
            certification.ExpiryDate,
            certification.Status,
            certification.DocumentId);
    }
}

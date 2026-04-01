using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class SupplierDocument
{
    public int Id { get; set; }

    public int SupplierId { get; set; }

    public string DocumentType { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string StoragePath { get; set; } = null!;

    public DateTime UploadedAt { get; set; }

    public string? UploadedBy { get; set; }

    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();

    public virtual Supplier Supplier { get; set; } = null!;
}

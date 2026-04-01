using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class Certification
{
    public int Id { get; set; }

    public int SupplierId { get; set; }

    public string Type { get; set; } = null!;

    public string Authority { get; set; } = null!;

    public string? Code { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public string Status { get; set; } = null!;

    public int? DocumentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual SupplierDocument? Document { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
}

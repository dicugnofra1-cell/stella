using System;

namespace Mandorle.Domain.Entities;

public partial class Invoice
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public string DocumentNumber { get; set; } = null!;

    public string DocumentType { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = null!;

    public DateTime IssueDate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}

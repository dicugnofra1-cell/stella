using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class NonConformity
{
    public int Id { get; set; }

    public int BatchId { get; set; }

    public string Severity { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? CorrectiveAction { get; set; }

    public string OpenedBy { get; set; } = null!;

    public DateTime OpenedAt { get; set; }

    public string? ClosedBy { get; set; }

    public DateTime? ClosedAt { get; set; }

    public virtual Batch Batch { get; set; } = null!;
}

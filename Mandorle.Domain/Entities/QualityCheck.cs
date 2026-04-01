using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class QualityCheck
{
    public int Id { get; set; }

    public int BatchId { get; set; }

    public string ChecklistVersion { get; set; } = null!;

    public string Result { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CheckedAt { get; set; }

    public string CheckedBy { get; set; } = null!;

    public virtual Batch Batch { get; set; } = null!;
}

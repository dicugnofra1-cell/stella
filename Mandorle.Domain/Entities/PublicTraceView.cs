using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class PublicTraceView
{
    public int BatchId { get; set; }

    public string BatchCode { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public bool BioFlag { get; set; }

    public string? OriginInfo { get; set; }

    public string? MainDates { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual Batch Batch { get; set; } = null!;
}

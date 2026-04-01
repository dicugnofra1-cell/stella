using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class AuditLog
{
    public long Id { get; set; }

    public string EntityName { get; set; } = null!;

    public string EntityId { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string ChangedBy { get; set; } = null!;

    public DateTime ChangedAt { get; set; }

    public string? CorrelationId { get; set; }
}

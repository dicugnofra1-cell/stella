using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class StockReservation
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int OrderItemId { get; set; }

    public int ProductId { get; set; }

    public int? BatchId { get; set; }

    public decimal Quantity { get; set; }

    public string Status { get; set; } = null!;

    public string ReservationType { get; set; } = null!;

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? ReleasedAt { get; set; }

    public DateTime? ConsumedAt { get; set; }

    public string? Notes { get; set; }

    public virtual Batch? Batch { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual OrderItem OrderItem { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

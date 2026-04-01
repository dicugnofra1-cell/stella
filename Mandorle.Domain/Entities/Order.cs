using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string OrderType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? PaymentStatus { get; set; }

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<StockReservation> StockReservations { get; set; } = new List<StockReservation>();
}

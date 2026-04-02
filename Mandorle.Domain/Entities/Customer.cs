using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class Customer
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? VatNumber { get; set; }

    public string Email { get; set; } = null!;

    public string? Pec { get; set; }

    public string? SpidIdentifier { get; set; }

    public string? Phone { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

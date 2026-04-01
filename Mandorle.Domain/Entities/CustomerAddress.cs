using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class CustomerAddress
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string AddressType { get; set; } = null!;

    public string RecipientName { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string? Street2 { get; set; }

    public string City { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string Country { get; set; } = null!;

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}

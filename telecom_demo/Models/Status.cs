using System;
using System.Collections.Generic;

namespace telecom_demo.Models;

public partial class Status
{
    public int IdStatus { get; set; }

    public string? NameStatus { get; set; }

    public virtual ICollection<OrderInsertsDelete> OrderInsertsDeletes { get; set; } = new List<OrderInsertsDelete>();

    public virtual ICollection<OrderUpdate> OrderUpdateNewStatusNavigations { get; set; } = new List<OrderUpdate>();

    public virtual ICollection<OrderUpdate> OrderUpdateOldStatusNavigations { get; set; } = new List<OrderUpdate>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

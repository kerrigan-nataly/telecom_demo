using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class Order
{
    public int IdOrder { get; set; }

    public string? NameOrder { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? DeadlineDate { get; set; }

    public int? StatusId { get; set; }

    public virtual ICollection<OrderComponent> OrderComponents { get; set; } = new List<OrderComponent>();

    public virtual ICollection<OrderInsertsDelete> OrderInsertsDeletes { get; set; } = new List<OrderInsertsDelete>();

    public virtual ICollection<OrderNode> OrderNodes { get; set; } = new List<OrderNode>();

    public virtual ICollection<OrderUpdate> OrderUpdates { get; set; } = new List<OrderUpdate>();

    public virtual Status? Status { get; set; }
}

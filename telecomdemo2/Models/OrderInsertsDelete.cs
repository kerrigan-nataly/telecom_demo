using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class OrderInsertsDelete
{
    public int IdHistory { get; set; }

    public string? OperationType { get; set; }

    public int? OrderId { get; set; }

    public int? OrderStatus { get; set; }

    public DateTime? ChangeDate { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Status? OrderStatusNavigation { get; set; }
}

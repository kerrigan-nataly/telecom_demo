using System;
using System.Collections.Generic;

namespace telecom_demo.Models;

public partial class OrderUpdate
{
    public int IdHistory { get; set; }

    public int? OrderId { get; set; }

    public int? OldStatus { get; set; }

    public int? NewStatus { get; set; }

    public DateTime? ChangeDate { get; set; }

    public virtual Status? NewStatusNavigation { get; set; }

    public virtual Status? OldStatusNavigation { get; set; }

    public virtual Order? Order { get; set; }
}

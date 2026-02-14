using System;
using System.Collections.Generic;

namespace telecom_demo.Models;

public partial class OrderComponent
{
    public int IdOrderComponent { get; set; }

    public int? OrderId { get; set; }

    public string? ComponentId { get; set; }

    public int? ComponentCount { get; set; }

    public virtual Component? Component { get; set; }

    public virtual Order? Order { get; set; }
}

using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class OrderNode
{
    public int IdOrderNode { get; set; }

    public int? OrderId { get; set; }

    public int? NodeId { get; set; }

    public int? NodeCount { get; set; }

    public virtual Node? Node { get; set; }

    public virtual Order? Order { get; set; }
}

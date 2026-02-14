using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class Node
{
    public int IdNode { get; set; }

    public string? NameNode { get; set; }

    public int? NodeTypeId { get; set; }

    public int? TestingResultId { get; set; }

    public virtual NodeType? NodeType { get; set; }

    public virtual ICollection<OrderNode> OrderNodes { get; set; } = new List<OrderNode>();

    public virtual TestingResult? TestingResult { get; set; }
}

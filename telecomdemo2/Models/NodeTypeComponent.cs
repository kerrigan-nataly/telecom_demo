using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class NodeTypeComponent
{
    public int IdNodeTypeComponent { get; set; }

    public int? NodeTypeId { get; set; }

    public string? ComponentId { get; set; }

    public int? ComponentCount { get; set; }

    public virtual Component? Component { get; set; }

    public virtual NodeType? NodeType { get; set; }
}

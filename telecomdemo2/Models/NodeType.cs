using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class NodeType
{
    public int IdNodeType { get; set; }

    public string? NameNodeType { get; set; }

    public virtual ICollection<NodeTypeComponent> NodeTypeComponents { get; set; } = new List<NodeTypeComponent>();

    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();
}

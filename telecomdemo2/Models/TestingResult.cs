using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class TestingResult
{
    public int IdTestingResult { get; set; }

    public string? NameTestingResult { get; set; }

    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();
}

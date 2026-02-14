using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class ComponentType
{
    public int IdComponentType { get; set; }

    public string? NameComponentType { get; set; }

    public virtual ICollection<Component> Components { get; set; } = new List<Component>();
}

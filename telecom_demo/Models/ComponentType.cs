using System;
using System.Collections.Generic;

namespace telecom_demo.Models;

public partial class ComponentType
{
    public int IdComponentType { get; set; }

    public string? NameComponentType { get; set; }

    public virtual ICollection<Component> Components { get; set; } = new List<Component>();
}

using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class ComponentStorage
{
    public int IdComponentStorage { get; set; }

    public string? ComponentId { get; set; }

    public int? StorageId { get; set; }

    public int? ComponentCount { get; set; }

    public virtual Component? Component { get; set; }

    public virtual Storage? Storage { get; set; }
}

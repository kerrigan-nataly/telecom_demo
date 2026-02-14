using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class Manufacturer
{
    public int IdManufacturer { get; set; }

    public string? NameManufacturer { get; set; }

    public string? Contact { get; set; }

    public int? Rating { get; set; }

    public TimeSpan? AvgTime { get; set; }

    public virtual ICollection<Component> Components { get; set; } = new List<Component>();
}

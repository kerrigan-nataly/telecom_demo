using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class ComponentStorageUpdate
{
    public int IdHistory { get; set; }

    public string? ComponentId { get; set; }

    public int? OldStorageId { get; set; }

    public int? NewStorageId { get; set; }

    public int? OldCount { get; set; }

    public int? NewCount { get; set; }

    public DateTime? ChangeDate { get; set; }

    public virtual Component? Component { get; set; }

    public virtual Storage? NewStorage { get; set; }

    public virtual Storage? OldStorage { get; set; }
}

using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class ComponentStorageInsertsDelete
{
    public int IdHistory { get; set; }

    public string? OperationType { get; set; }

    public string? ComponentId { get; set; }

    public int? StorageId { get; set; }

    public int? ComponentCount { get; set; }

    public DateTime? ChangeDate { get; set; }

    public virtual Component? Component { get; set; }

    public virtual Storage? Storage { get; set; }
}

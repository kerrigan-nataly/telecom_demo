using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class StorageType
{
    public int IdStorageType { get; set; }

    public string? NameStorageType { get; set; }

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}

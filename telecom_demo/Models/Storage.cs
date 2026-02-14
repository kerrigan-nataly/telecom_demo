using System;
using System.Collections.Generic;

namespace telecom_demo.Models;

public partial class Storage
{
    public int IdStorage { get; set; }

    public string? NameStorage { get; set; }

    public int? StorageTypeId { get; set; }

    public int? TemperatureModeId { get; set; }

    public virtual ICollection<ComponentStorageInsertsDelete> ComponentStorageInsertsDeletes { get; set; } = new List<ComponentStorageInsertsDelete>();

    public virtual ICollection<ComponentStorageUpdate> ComponentStorageUpdateNewStorages { get; set; } = new List<ComponentStorageUpdate>();

    public virtual ICollection<ComponentStorageUpdate> ComponentStorageUpdateOldStorages { get; set; } = new List<ComponentStorageUpdate>();

    public virtual ICollection<ComponentStorage> ComponentStorages { get; set; } = new List<ComponentStorage>();

    public virtual StorageType? StorageType { get; set; }

    public virtual TemperatureMode? TemperatureMode { get; set; }
}

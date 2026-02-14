using System;
using System.Collections.Generic;

namespace telecom_demo.Models;

public partial class Component
{
    public string IdComponent { get; set; } = null!;

    public string? NameComponent { get; set; }

    public int? ComponentTypeId { get; set; }

    public int? ManufacturerId { get; set; }

    public string? Characteristic { get; set; }

    public string? Unit { get; set; }

    public int? MinStorage { get; set; }

    public int? MaxStorage { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public int? TemperatureModeId { get; set; }

    public string? PathToScheme { get; set; }

    public virtual ICollection<ComponentStorageInsertsDelete> ComponentStorageInsertsDeletes { get; set; } = new List<ComponentStorageInsertsDelete>();

    public virtual ICollection<ComponentStorageUpdate> ComponentStorageUpdates { get; set; } = new List<ComponentStorageUpdate>();

    public virtual ICollection<ComponentStorage> ComponentStorages { get; set; } = new List<ComponentStorage>();

    public virtual ComponentType? ComponentType { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual ICollection<OrderComponent> OrderComponents { get; set; } = new List<OrderComponent>();

    public virtual TemperatureMode? TemperatureMode { get; set; }
}

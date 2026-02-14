using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class TemperatureMode
{
    public int IdTemperatureMode { get; set; }

    public string? NameTemperatureMode { get; set; }

    public int? MinTemperature { get; set; }

    public int? MaxTemperature { get; set; }

    public int? MinHumidity { get; set; }

    public int? MaxHumidity { get; set; }

    public virtual ICollection<Component> Components { get; set; } = new List<Component>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}

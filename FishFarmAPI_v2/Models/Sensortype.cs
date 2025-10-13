using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Sensortype
{
    public int SensorTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Status { get; set; }

    public virtual ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
}

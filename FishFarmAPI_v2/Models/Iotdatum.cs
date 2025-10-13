using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Iotdatum
{
    public int DataId { get; set; }

    public int SensorId { get; set; }

    public string? Value { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Sensor Sensor { get; set; } = null!;
}

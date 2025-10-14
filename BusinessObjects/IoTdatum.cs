using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class IoTdatum
{
    public int DataId { get; set; }

    public int SensorId { get; set; }

    public string? Value { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Sensor Sensor { get; set; } = null!;
}

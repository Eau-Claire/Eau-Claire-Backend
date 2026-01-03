using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class SensorsType
{
    public int SensorTypeId { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
}

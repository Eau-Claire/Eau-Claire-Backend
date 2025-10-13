using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Sensor
{
    public int SensorId { get; set; }

    public int SensorTypeId { get; set; }

    public string? Status { get; set; }

    public int? AssignedToTank { get; set; }

    public int? AssignedToCluster { get; set; }

    public virtual Tankcluster? AssignedToClusterNavigation { get; set; }

    public virtual Tank? AssignedToTankNavigation { get; set; }

    public virtual ICollection<Iotdatum> Iotdata { get; set; } = new List<Iotdatum>();

    public virtual Sensortype SensorType { get; set; } = null!;
}

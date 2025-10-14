using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class Sensor
{
    public int SensorId { get; set; }

    public int SensorTypeId { get; set; }

    public string? Status { get; set; }

    public int? AssignedToTank { get; set; }

    public int? AssignedToCluster { get; set; }

    public virtual TankCluster? AssignedToClusterNavigation { get; set; }

    public virtual Tank? AssignedToTankNavigation { get; set; }

    public virtual ICollection<IoTdatum> IoTdata { get; set; } = new List<IoTdatum>();

    public virtual SensorType SensorType { get; set; } = null!;
}

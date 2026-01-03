using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class Sensor
{
    public int SensorId { get; set; }

    public int SensorTypeId { get; set; }

    public int? AssignedToTank { get; set; }

    public int? AssignedToCluster { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual TankCluster? AssignedToClusterNavigation { get; set; }

    public virtual Tank? AssignedToTankNavigation { get; set; }

    public virtual SensorsType SensorType { get; set; } = null!;
}

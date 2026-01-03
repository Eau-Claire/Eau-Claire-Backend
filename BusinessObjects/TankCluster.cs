using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class TankCluster
{
    public int ClusterId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();

    public virtual ICollection<Tank> Tanks { get; set; } = new List<Tank>();
}

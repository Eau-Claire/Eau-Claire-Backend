using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Tank
{
    public int TankId { get; set; }

    public string Name { get; set; } = null!;

    public int? Quantity { get; set; }

    public int? ClusterId { get; set; }

    public int? BreedId { get; set; }

    public virtual Fishbreed? Breed { get; set; }

    public virtual Tankcluster? Cluster { get; set; }

    public virtual ICollection<Fishhealthstatus> Fishhealthstatuses { get; set; } = new List<Fishhealthstatus>();

    public virtual ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
}

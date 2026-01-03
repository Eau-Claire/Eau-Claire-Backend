using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class Tank
{
    public int TankId { get; set; }

    public string? Name { get; set; }

    public int? Quantity { get; set; }

    public int? ClusterId { get; set; }

    public int? BreedId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual FishBreed? Breed { get; set; }

    public virtual TankCluster? Cluster { get; set; }

    public virtual ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
}

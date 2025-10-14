using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class FishHealthStatus
{
    public int StatusId { get; set; }

    public int TankId { get; set; }

    public string? CurrentStatus { get; set; }

    public string? Prediction { get; set; }

    public virtual Tank Tank { get; set; } = null!;
}

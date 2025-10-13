using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Fishhealthstatus
{
    public int StatusId { get; set; }

    public int TankId { get; set; }

    public string? CurrentStatus { get; set; }

    public string? Prediction { get; set; }

    public virtual Tank Tank { get; set; } = null!;
}

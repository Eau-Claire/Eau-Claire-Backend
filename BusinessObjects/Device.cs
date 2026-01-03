using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class Device
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? DeviceName { get; set; }

    public string DeviceIdentifier { get; set; } = null!;

    public string? DeviceType { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiredAt { get; set; }

    public bool IsVerified { get; set; }

    public virtual User User { get; set; } = null!;
}

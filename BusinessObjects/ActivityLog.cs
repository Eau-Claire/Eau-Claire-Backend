using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class ActivityLog
{
    public long LogId { get; set; }

    public int? UserId { get; set; }

    public string? Action { get; set; }

    public string? TargetType { get; set; }

    public int? TargetId { get; set; }

    public string? Description { get; set; }

    public string? IpAddress { get; set; }

    public DateTime? CreatedAt { get; set; }
}

using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string? Type { get; set; }

    public virtual User User { get; set; } = null!;
}

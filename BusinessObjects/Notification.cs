using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string? Type { get; set; }

    public virtual User User { get; set; } = null!;
}

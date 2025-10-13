using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Shopstaff
{
    public int StaffId { get; set; }

    public int UserId { get; set; }

    public string? Permissions { get; set; }

    public virtual User User { get; set; } = null!;
}

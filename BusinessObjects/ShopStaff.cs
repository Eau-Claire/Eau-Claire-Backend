using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class ShopStaff
{
    public int StaffId { get; set; }

    public int UserId { get; set; }

    public string? Permissions { get; set; }

    public virtual User User { get; set; } = null!;
}

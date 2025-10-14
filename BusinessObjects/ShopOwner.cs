using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class ShopOwner
{
    public int OwnerId { get; set; }

    public int UserId { get; set; }

    public string? Address { get; set; }

    public virtual User User { get; set; } = null!;
}

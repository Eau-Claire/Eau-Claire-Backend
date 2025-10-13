using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Shopowner
{
    public int OwnerId { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}

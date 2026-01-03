using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class UserStoreRole
{
    public int UserId { get; set; }

    public int StoreId { get; set; }

    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

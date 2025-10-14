using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class Permission
{
    public int PermissionId { get; set; }

    public string Name { get; set; } = null!;
}

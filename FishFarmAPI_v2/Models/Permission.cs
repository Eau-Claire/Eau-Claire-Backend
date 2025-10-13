using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Permission
{
    public int PermissionId { get; set; }

    public string Name { get; set; } = null!;
}

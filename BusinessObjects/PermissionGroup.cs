using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class PermissionGroup
{
    public int GroupId { get; set; }

    public string Name { get; set; } = null!;
}

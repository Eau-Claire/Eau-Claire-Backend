using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class Permission
{
    public int PermissionId { get; set; }

    public int GroupId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual PermissionGroup Group { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}

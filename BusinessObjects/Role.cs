using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class Role
{
    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<UserStoreRole> UserStoreRoles { get; set; } = new List<UserStoreRole>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}

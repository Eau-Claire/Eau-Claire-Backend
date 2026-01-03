using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class Store
{
    public int StoreId { get; set; }

    public string StoreName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<UserStoreRole> UserStoreRoles { get; set; } = new List<UserStoreRole>();
}

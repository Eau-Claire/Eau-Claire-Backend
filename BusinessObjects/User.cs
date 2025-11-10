using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FishFarm.BusinessObjects;

public partial class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Role { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<ShopOwner> ShopOwners { get; set; } = new List<ShopOwner>();

    public virtual ICollection<ShopStaff> ShopStaffs { get; set; } = new List<ShopStaff>();

    public virtual ICollection<SpendingRecord> SpendingRecords { get; set; } = new List<SpendingRecord>();

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}

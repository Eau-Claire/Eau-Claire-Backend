using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FishFarm.BusinessObjects;

public partial class User
{
    [Column("UserId")]
    public int UserId { get; set; }
    [Column("Username")]
    public string Username { get; set; } = null!;
    [Column("PasswordHash")]
    public string PasswordHash { get; set; } = null!;
    [Column("Email")]
    public string? Email { get; set; }
    [Column("Phone")]
    public string? Phone { get; set; }
    [Column("Role")]
    public string? Role { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<ShopOwner> ShopOwners { get; set; } = new List<ShopOwner>();

    public virtual ICollection<ShopStaff> ShopStaffs { get; set; } = new List<ShopStaff>();

    public virtual ICollection<SpendingRecord> SpendingRecords { get; set; } = new List<SpendingRecord>();

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}

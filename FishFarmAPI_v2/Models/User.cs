using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Shopowner> Shopowners { get; set; } = new List<Shopowner>();

    public virtual ICollection<Shopstaff> Shopstaffs { get; set; } = new List<Shopstaff>();

    public virtual ICollection<Spendingrecord> Spendingrecords { get; set; } = new List<Spendingrecord>();
}

using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class UserProfile
{
    public int UserProfileId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public DateTime? Dob { get; set; }

    public string? Gender { get; set; }

    public virtual User User { get; set; } = null!;
}

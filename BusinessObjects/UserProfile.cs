using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class UserProfile
{
    public int ProfileId { get; set; }

    public int UserId { get; set; }

    public string? FullName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? CurrentPhoneNumber { get; set; }

    public string? PermanentAddress { get; set; }

    public string? ContactAddress { get; set; }

    public virtual User User { get; set; } = null!;
}

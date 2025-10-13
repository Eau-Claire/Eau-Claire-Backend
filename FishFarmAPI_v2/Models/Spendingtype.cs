using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Spendingtype
{
    public int SpendingTypeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Spendingrecord> Spendingrecords { get; set; } = new List<Spendingrecord>();
}

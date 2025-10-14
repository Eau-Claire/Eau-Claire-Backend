﻿using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class SpendingType
{
    public int SpendingTypeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<SpendingRecord> SpendingRecords { get; set; } = new List<SpendingRecord>();
}

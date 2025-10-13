using System;
using System.Collections.Generic;

namespace FishFarmAPI_v2.Models;

public partial class Spendingrecord
{
    public int RecordId { get; set; }

    public int UserId { get; set; }

    public int SpendingTypeId { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string? AccountNo { get; set; }

    public string? BankNo { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Spendingtype SpendingType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

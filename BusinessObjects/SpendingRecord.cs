using System;
using System.Collections.Generic;

namespace FishFarm.BusinessObjects;

public partial class SpendingRecord
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

    public virtual SpendingType SpendingType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

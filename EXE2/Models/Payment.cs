using System;
using System.Collections.Generic;

namespace EXE2.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime? PaidAt { get; set; }

    public string? Status { get; set; }

    public virtual Order Order { get; set; } = null!;
}

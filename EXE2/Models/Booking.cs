using System;
using System.Collections.Generic;

namespace EXE2.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int TableId { get; set; }

    public DateTime BookingTime { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Table Table { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

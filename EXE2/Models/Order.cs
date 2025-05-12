using System;
using System.Collections.Generic;

namespace EXE2.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int? BookingId { get; set; }

    public DateTime? OrderTime { get; set; }

    public string? Status { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}

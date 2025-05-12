using System;
using System.Collections.Generic;

namespace EXE2.Models;

public partial class Table
{
    public int TableId { get; set; }

    public string TableName { get; set; } = null!;

    public int Capacity { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

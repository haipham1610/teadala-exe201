using System;
using System.Collections.Generic;

namespace EXE2.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role RoleNavigation { get; set; } = null!;
}

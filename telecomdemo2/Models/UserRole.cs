using System;
using System.Collections.Generic;

namespace telecomdemo2.Models;

public partial class UserRole
{
    public int IdUserRole { get; set; }

    public string? NameUserRole { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

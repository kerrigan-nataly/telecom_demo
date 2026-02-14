using System;
using System.Collections.Generic;

namespace telecom_demo.Models;

public partial class User
{
    public int IdUser { get; set; }

    public int? UserRoleId { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? Fullname { get; set; }

    public virtual UserRole? UserRole { get; set; }
}

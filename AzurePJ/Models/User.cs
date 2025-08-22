using System;
using System.Collections.Generic;

namespace AzurePJ.Models;

public partial class User
{
    public long UserId { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string Password { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime? LastLoginTime { get; set; }

    public string? LastLoginIp { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

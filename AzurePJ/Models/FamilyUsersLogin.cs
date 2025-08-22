using System;
using System.Collections.Generic;

namespace AzurePJ.Models;

public partial class FamilyUsersLogin
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Relationship { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }
}

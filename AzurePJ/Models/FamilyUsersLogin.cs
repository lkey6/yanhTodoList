using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AzurePJ.Models;

public partial class FamilyUsersLogin
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "名前を入力してください")]
    public string UserName { get; set; } = null!;

    public string Relationship { get; set; } = null!;

    [Required(ErrorMessage = "Passwordを入力してください")]
    public string Password { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }
}

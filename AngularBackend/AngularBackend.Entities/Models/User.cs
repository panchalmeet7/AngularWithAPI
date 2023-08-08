using System;
using System.Collections.Generic;

namespace AngularBackend.Entities.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public long PhoneNumber { get; set; }

    public string Password { get; set; } = null!;

    public string? Token { get; set; }

    public string? Role { get; set; }

    public string? ResetPassewordToken { get; set; }

    public DateTime? ResetPasswordExpiry { get; set; }
}

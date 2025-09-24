using System;
using System.Collections.Generic;

namespace CungCapAPI.Domain.Models;

public partial class TbUserLogin
{
    public int UserId { get; set; }

    public string AccountLogin { get; set; } = null!;

    public byte[] PasswordLogin { get; set; } = null!;

    public byte[] SaltLogin { get; set; } = null!;

    public DateTime? LastLoginAt { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual TbUser User { get; set; } = null!;
}

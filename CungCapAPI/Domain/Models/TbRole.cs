using System;
using System.Collections.Generic;

namespace CungCapAPI.Domain.Models;

public partial class TbRole
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string RoleScope { get; set; } = null!;

    public string? RoleDescribe { get; set; }

    public virtual ICollection<TbUserDeviceRole> TbUserDeviceRoles { get; set; } = new List<TbUserDeviceRole>();

    public virtual ICollection<TbUser> TbUsers { get; set; } = new List<TbUser>();
}

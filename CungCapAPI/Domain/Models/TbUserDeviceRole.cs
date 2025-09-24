using System;
using System.Collections.Generic;

namespace CungCapAPI.Domain.Models;

public partial class TbUserDeviceRole
{
    public int UserId { get; set; }

    public int DeviceId { get; set; }

    public int RoleId { get; set; }

    public virtual TbDevice Device { get; set; } = null!;

    public virtual TbRole Role { get; set; } = null!;

    public virtual TbUser User { get; set; } = null!;
}

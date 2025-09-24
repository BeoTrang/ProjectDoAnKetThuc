using System;
using System.Collections.Generic;

namespace CungCapAPI.Models.SqlServer;

public partial class TbAlert
{
    public int AlertId { get; set; }

    public int DeviceId { get; set; }

    public string Condition { get; set; } = null!;

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? TriggeredAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public int? ResolvedBy { get; set; }

    public virtual TbDevice Device { get; set; } = null!;

    public virtual TbUser? ResolvedByNavigation { get; set; }
}

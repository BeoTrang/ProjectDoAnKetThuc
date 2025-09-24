using System;
using System.Collections.Generic;

namespace CungCapAPI.Models.SqlServer;

public partial class TbCommand
{
    public int CommandId { get; set; }

    public int DeviceId { get; set; }

    public int UserId { get; set; }

    public string Command { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual TbDevice Device { get; set; } = null!;

    public virtual TbUser User { get; set; } = null!;
}

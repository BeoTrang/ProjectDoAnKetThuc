using System;
using System.Collections.Generic;

namespace CungCapAPI.Models.SqlServer;

public partial class TbDevice
{
    public int DeviceId { get; set; }

    public string Name { get; set; } = null!;

    public string TypeId { get; set; } = null!;

    public string? Status { get; set; }

    public string? FirmwareVersion { get; set; }

    public DateTime? RegisteredAt { get; set; }

    public virtual ICollection<TbAlert> TbAlerts { get; set; } = new List<TbAlert>();

    public virtual ICollection<TbCommand> TbCommands { get; set; } = new List<TbCommand>();

    public virtual ICollection<TbNotification> TbNotifications { get; set; } = new List<TbNotification>();

    public virtual ICollection<TbUserDeviceRole> TbUserDeviceRoles { get; set; } = new List<TbUserDeviceRole>();

    public virtual TbDeviceType Type { get; set; } = null!;
}

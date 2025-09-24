using System;
using System.Collections.Generic;

namespace CungCapAPI.Domain.Models;

public partial class TbUser
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string? PhoneNumber { get; set; }

    public int? GlobalRoleId { get; set; }

    public virtual TbRole? GlobalRole { get; set; }

    public virtual ICollection<TbAlert> TbAlerts { get; set; } = new List<TbAlert>();

    public virtual ICollection<TbCommand> TbCommands { get; set; } = new List<TbCommand>();

    public virtual ICollection<TbNotification> TbNotificationReceivers { get; set; } = new List<TbNotification>();

    public virtual ICollection<TbNotification> TbNotificationSenders { get; set; } = new List<TbNotification>();

    public virtual ICollection<TbUserDeviceRole> TbUserDeviceRoles { get; set; } = new List<TbUserDeviceRole>();

    public virtual TbUserLogin? TbUserLogin { get; set; }

    public virtual TbUserTelegram? TbUserTelegram { get; set; }
}

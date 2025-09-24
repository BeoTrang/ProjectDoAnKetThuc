using System;
using System.Collections.Generic;

namespace CungCapAPI.Models.SqlServer;

public partial class TbNotification
{
    public int NotificationId { get; set; }

    public string NotificationType { get; set; } = null!;

    public int? DeviceId { get; set; }

    public int? SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual TbDevice? Device { get; set; }

    public virtual TbUser? Receiver { get; set; }

    public virtual TbUser? Sender { get; set; }
}

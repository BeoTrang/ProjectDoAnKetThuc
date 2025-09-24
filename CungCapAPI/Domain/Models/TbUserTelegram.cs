using System;
using System.Collections.Generic;

namespace CungCapAPI.Domain.Models;

public partial class TbUserTelegram
{
    public int UserId { get; set; }

    public string TeleChatId { get; set; } = null!;

    public string TeleBotId { get; set; } = null!;

    public virtual TbUser User { get; set; } = null!;
}

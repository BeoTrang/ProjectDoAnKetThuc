using System;
using System.Collections.Generic;

namespace CungCapAPI.Domain.Models;

public partial class TbDeviceType
{
    public string TypeId { get; set; } = null!;

    public string NameType { get; set; } = null!;

    public virtual ICollection<TbDevice> TbDevices { get; set; } = new List<TbDevice>();
}

using System;
using System.Collections.Generic;

namespace CungCapAPI.Models;

public partial class VaiTro
{
    public int VaiTroId { get; set; }

    public string VaiTro1 { get; set; } = null!;

    public string? MieuTa { get; set; }

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}

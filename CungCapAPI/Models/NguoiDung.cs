using System;
using System.Collections.Generic;

namespace CungCapAPI.Models;

public partial class NguoiDung
{
    public int NguoiDungId { get; set; }

    public bool KichHoat { get; set; }

    public string TenNguoiDung { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string TaiKhoan { get; set; } = null!;

    public string MatKhauMaHoa { get; set; } = null!;

    public string MatKhauMuoi { get; set; } = null!;

    public DateTime TaoVao { get; set; }

    public DateTime? CapNhatVao { get; set; }

    public DateTime? LanCuoiDangNhap { get; set; }

    public int DangNhaploi { get; set; }

    public DateTime? KhoaKetThuc { get; set; }

    public virtual ICollection<VaiTro> VaiTros { get; set; } = new List<VaiTro>();
}

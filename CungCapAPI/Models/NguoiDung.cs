using System;
using System.Collections.Generic;

namespace CungCapAPI.Models;

public partial class NguoiDung
{
    public int NguoiDungId { get; set; }

    public bool? KichHoat { get; set; }

    public string? TenNguoiDung { get; set; }

    public string? Email { get; set; }

    public string? TaiKhoan { get; set; }

    public byte[]? MatKhauMaHoa { get; set; }

    public byte[]? MatKhauMuoi { get; set; }

    public DateTime? TaoVao { get; set; }

    public DateTime? CapNhatVao { get; set; }

    public DateTime? LanCuoiDangNhap { get; set; }

    public int? DangNhaploi { get; set; }

    public DateTime? KhoaKetThuc { get; set; }

    public virtual ICollection<VaiTro> VaiTros { get; set; } = new List<VaiTro>();
}

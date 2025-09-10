using Microsoft.EntityFrameworkCore;

namespace CungCapAPI.Models
{
    [Keyless]
    public class TaiKhoanGuiVe
    {
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
    }
    [Keyless]
    public class RefreshRequest
    {
        public string RefreshToken { get; set; }
    }
    [Keyless]
    public class ThongTinNguoiDung
    {
        public int NguoiDungId { get; set; }
        public string TenNguoiDung { get; set; }
        public string Email { get; set; }
        public string VaiTro { get; set; }
    }
}

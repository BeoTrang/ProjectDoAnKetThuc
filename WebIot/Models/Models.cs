namespace WebIot.Models
{ 
    public class TaiKhoanGuiVe
    {
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
    }
    public class PhanHoiApi<T>
    {
        public bool success { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }
    public class JWT
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
    public class RefreshRequest
    {
        public string refreshToken { get; set; }
    }
    public class ThongTinNguoiDung
    {
        public string nguoiDungId { get; set; }
        public string tenNguoiDung { get; set; }
        public string email { get; set; }
        public string vaiTro { get; set; }
    }
    
}

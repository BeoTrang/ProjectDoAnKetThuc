namespace WebIot.Models
{ 
    public class TaiKhoanGuiVe
    {
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
    }
    public class TaiKhoanDangKy
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string account_login { get; set; }
        public string password_login { get; set; }  
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
        public int NguoiDungId { get; set; }
        public string TenNguoiDung { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string VaiTro { get; set; }
    }
    public class HoSoTaiKhoan
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string account_login { get; set; }
        public string tele_chat_id { get; set; }
        public string tele_bot_id { get; set; }
    }
}

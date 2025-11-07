namespace ModelLibrary
{
    public class TaiKhoanGuiVe
    {
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
    }
    public class RefreshRequest
    {
        public string RefreshToken { get; set; }
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
    public class DoiMatKhau
    {
        public string matKhauCu { get; set; }
        public string matKhauMoi { get; set; }
    }
    public class CaiDatTelegram
    {
        public string chatId { get; set; }
        public string botId { get; set; }
    }
    public class CaiDatThongTinTaiKhoan
    {
        public string hoVaTen { get; set; }
        public string email { get; set; }
        public string soDienThoai { get; set; }
        public string taiKhoanDangNhap { get; set; }
    }
    public class KiemTraJWT
    {
        public bool success { get; set; }
        public string accessToken { get; set; }
    }
    public class ShareRequest
    {
        public int deviceid { get; set; }
        public string quyen { get; set; }
    }

}

namespace CungCapAPI.Helpers
{
    public static class PasswordHelper
    {
        public static bool KiemTraMatKhau(string MatKhau, string MaKhauDB, string MuoiDB)
        {
            return MaKhauDB == MatKhau;
        }
    }

}

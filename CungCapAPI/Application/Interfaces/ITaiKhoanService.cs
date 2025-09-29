using CungCapAPI.Models.DTO;

namespace CungCapAPI.Application.Interfaces
{
    public interface ITaiKhoanService
    {
        Task<LoginResult<jwtTokens>> CapJTW(int NguoiDungId);
        Task<int> KiemTraMatKhau(string TaiKhoan, string MatKhau);
        Task<bool> XoaRefreshToken(string RefreshToken);
        Task<LoginResult<ThongTinNguoiDung>> LayThongTinNguoiDung(int NguoiDungId);
        Task<LoginResult<jwtTokens>> CapLaiAccessToken(string refreshToken);
        Task<bool> KiemTraTaiKhoanTonTai(string email, string phone_number, string account_login);
        Task<bool> DangKyTaiKhoan(DangKyTaiKhoan request);
        Task<Request<HoSoTaiKhoan>> LayHoSoTaiKhoan(int nguoiDungId);
    }
}

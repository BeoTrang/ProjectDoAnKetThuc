using CungCapAPI.Application.Interfaces;
using CungCapAPI.Helpers;
using CungCapAPI.Models.DichVuTrong;
using CungCapAPI.Models.DTO;

namespace CungCapAPI.Application.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly TaiKhoanRepository _taiKhoanRepository;
        private readonly IConfiguration _config;

        public TaiKhoanService(TaiKhoanRepository taiKhoanRepository,IConfiguration config)
        {
            _taiKhoanRepository = taiKhoanRepository;
            _config = config;
        }

        public async Task<LoginResult<jwtTokens>> CapJTW(int NguoiDungId)
        {
            var user = await _taiKhoanRepository.LayThongTinNguoiDung(NguoiDungId);

            if (user == null)
            {
                return new LoginResult<jwtTokens>
                {
                    success = false,
                    message = "Người dùng không tồn tại!"
                };
            }

            //Tạo access token và refresh token
            var accessToken = JwtHelper.GenerateAccessToken(user, _config);
            var refreshToken = JwtHelper.GenerateRefreshToken();    

            //Lưu refresh token vào Redis với thời gian hết hạn là 7 ngày
            var result = await _taiKhoanRepository.LuuRefreshToken(user.NguoiDungId, refreshToken, TimeSpan.FromDays(7));
            if (!result)
            {
                return new LoginResult<jwtTokens>
                {
                    success = false,
                    message = "Lỗi lưu refreshToken!"
                };
            }

            return new LoginResult<jwtTokens>
            {
                success = true,
                message = "Lấy thông tin người dùng thành công!",
                data = new jwtTokens
                {
                    accessToken = accessToken,
                    refreshToken = refreshToken
                }
            };
        }
        public async Task<LoginResult<ThongTinNguoiDung>> LayThongTinNguoiDung(int nguoiDungId)
        {
            var nguoiDung = await _taiKhoanRepository.LayThongTinNguoiDung(nguoiDungId);

            if (nguoiDung == null)
            {
                return new LoginResult<ThongTinNguoiDung>
                {
                    success = false,
                    message = "Người dùng không tồn tại!"
                };
            }

            return new LoginResult<ThongTinNguoiDung>
            {
                success = true,
                message = "Lấy thông tin thành công!",
                data = nguoiDung
            };
        }

        public async Task<LoginResult<jwtTokens>> CapLaiAccessToken(string refreshToken)
        {
            int NguoiDungId = await _taiKhoanRepository.KiemTraRefreshToken(refreshToken);
            if (NguoiDungId == 0)
            {
                return new LoginResult<jwtTokens>
                {
                    success = false,
                    message = "Không tìm thấy RefreshToken!"
                };
            }
            else
            {
                var user = await _taiKhoanRepository.LayThongTinNguoiDung(NguoiDungId);
                var accessToken = JwtHelper.GenerateAccessToken(user, _config);
                return new LoginResult<jwtTokens>
                {
                    success = true,
                    message = "Đã cấp lại AccessToken!",
                    data = new jwtTokens
                    {
                        accessToken = accessToken
                    }
                };
            }
        }

        public async Task<bool> KiemTraTaiKhoanTonTai(string email, string phone_number, string account_login)
        {
            int KetQua = await _taiKhoanRepository.KiemTraTaiKhoanTonTai(email, phone_number, account_login);
            if(KetQua != 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<int> KiemTraMatKhau(string TaiKhoan, string MatKhau)
        {
            string KeyPepper = _config["Key:KeyPepper"] ?? "";
            return await _taiKhoanRepository.KiemTraMatKhau(TaiKhoan, MatKhau, KeyPepper);
        }
        public async Task<bool> XoaRefreshToken(string refreshToken)
        {
            return await _taiKhoanRepository.XoaRefreshToken(refreshToken);
        }
        public async Task<bool> DangKyTaiKhoan(DangKyTaiKhoan request)
        {
            string KeyPepper = _config["Key:KeyPepper"] ?? "";
            return await _taiKhoanRepository.DangKyTaiKhoan(request, KeyPepper);
        }
    }
}

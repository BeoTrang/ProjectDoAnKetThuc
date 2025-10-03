using CungCapAPI.Models.Redis;
using CungCapAPI.Models.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ModelLibrary;
using System.Linq;

namespace CungCapAPI.Models.DichVuTrong
{
    public class TaiKhoanRepository
    {
        private readonly ApplicationDbContext _SqlServer;
        private readonly IRedisService _Redis;

        public TaiKhoanRepository(ApplicationDbContext SqlServer, IRedisService Redis)
        {
            _SqlServer = SqlServer;
            _Redis = Redis;
        }
        public async Task<int> KiemTraMatKhau(string TaiKhoan, string MatKhau, string KeyPepper)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<int>("EXEC SP_KiemTraMatKhau @TaiKhoanCanCheck, @MatKhauCanCheck, @SuperKey",
                    new SqlParameter("@TaiKhoanCanCheck", TaiKhoan),
                    new SqlParameter("@MatKhauCanCheck", MatKhau),
                    new SqlParameter("@SuperKey", KeyPepper)
                )
                .ToListAsync();
            return result.FirstOrDefault();
        }
        public async Task<ThongTinNguoiDung> LayThongTinNguoiDung(int NguoiDungId)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<ThongTinNguoiDung>("EXEC SP_GetInfoUserForAccessToken @NguoiDungId",
                    new SqlParameter("@NguoiDungId", NguoiDungId)
                )
                .ToListAsync();
            return result.FirstOrDefault();
        }
        public async Task<HoSoTaiKhoan> LayHoSoTaiKhoan(int NguoiDungId)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<HoSoTaiKhoan>("EXEC SP_HoSoTaiKhoan @NguoiDungId",
                    new SqlParameter("@NguoiDungId", NguoiDungId)
                )
                .ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<int> KiemTraTaiKhoanTonTai(string email, string phone_number, string account_login)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<int>("EXEC SP_KiemTraTaiKhoanTonTai @email, @phone_number, @account_login",
                    new SqlParameter("@email", email),
                    new SqlParameter("@phone_number", phone_number),
                    new SqlParameter("@account_login", account_login)
                )
                .ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<bool> LuuRefreshToken(int NguoiDungId, string RefreshToken, TimeSpan expiry)
        {
            try
            {
                await _Redis.SetAsync($"refresh_token:{RefreshToken}", NguoiDungId.ToString(), expiry);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> XoaRefreshToken(string RefreshToken)
        {
            try
            {
                await _Redis.RemoveAsync($"refresh_token:{RefreshToken}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> KiemTraRefreshToken(string RefreshToken)
        {
            try
            {
                return int.Parse(await _Redis.GetAsync($"refresh_token:{RefreshToken}"));
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> DangKyTaiKhoan(DangKyTaiKhoan request, string KeyPepper)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<int>("EXEC SP_DangKyTaiKhoan @name, @email, @phone_number, @account_login, @password_login, @superKey",
                    new SqlParameter("@name", request.name),
                    new SqlParameter("@email", request.email),
                    new SqlParameter("@phone_number", request.phone_number),
                    new SqlParameter("@account_login", request.account_login),
                    new SqlParameter("@password_login", request.password_login),
                    new SqlParameter("@superKey", KeyPepper)
                )
                .ToListAsync();
            if (result.FirstOrDefault() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<int> KiemTraVaDoiMatKhau(int NguoiDungId, string MatKhauCu, string MatKhauMoi, string KeyPepper)
        {
            try
            {
                var result = await _SqlServer.Database
                .SqlQueryRaw<int>("EXEC SP_KiemTraVaDoiMatKhau @NguoiDungId, @MatKhauCu, @MatKhauMoi, @superKey",
                    new SqlParameter("@NguoiDungId", NguoiDungId),
                    new SqlParameter("@MatKhauCu", MatKhauCu),
                    new SqlParameter("@MatKhauMoi", MatKhauMoi),
                    new SqlParameter("@superKey", KeyPepper)
                )
                .ToListAsync();
                return result.FirstOrDefault();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> DoiThongTinTelegram(int NguoiDungId, CaiDatTelegram model)
        {
            try
            {
                var result = await _SqlServer.Database
                .SqlQueryRaw<int>("EXEC SP_DoiThongTinTelegram @NguoiDungId, @ChatId, @BotId",
                    new SqlParameter("@NguoiDungId", NguoiDungId),
                    new SqlParameter("@ChatId", model.chatId),
                    new SqlParameter("@BotId", model.botId)
                )
                .ToListAsync();
                return result.FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> DoiThongTinNguoiDung(int NguoiDungId, CaiDatThongTinTaiKhoan model)
        {
            try
            {
                var result = await _SqlServer.Database
                .SqlQueryRaw<int>("EXEC SP_DoiThongTinNguoiDung @NguoiDungId, @HoVaTen, @Email, @SoDienThoai, @TaiKhoanDangNhap",
                    new SqlParameter("@NguoiDungId", NguoiDungId),
                    new SqlParameter("@HoVaTen", model.hoVaTen),
                    new SqlParameter("@Email", model.email),
                    new SqlParameter("@SoDienThoai", model.soDienThoai),
                    new SqlParameter("@TaiKhoanDangNhap", model.taiKhoanDangNhap)
                )
                .ToListAsync();
                return result.FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
    }
}

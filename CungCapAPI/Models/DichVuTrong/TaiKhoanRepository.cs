using CungCapAPI.Models.DTO;
using CungCapAPI.Models.Redis;
using CungCapAPI.Models.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CungCapAPI.Application.Interfaces;

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
    }
}

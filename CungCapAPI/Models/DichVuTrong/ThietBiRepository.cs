using CungCapAPI.Models.Redis;
using CungCapAPI.Models.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ModelLibrary;
using System.Linq;

namespace CungCapAPI.Models.DichVuTrong
{
    public class ThietBiRepository
    {
        private readonly ApplicationDbContext _SqlServer;
        private readonly IRedisService _Redis;

        public ThietBiRepository(ApplicationDbContext SqlServer, IRedisService Redis)
        {
            _SqlServer = SqlServer;
            _Redis = Redis;
        }
        public async Task<List<DanhSachThietBi>> DanhSachThietBi(int NguoiDungId)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<DanhSachThietBi>("EXEC SP_LayDanhSachThietBi @NguoiDungId",
                    new SqlParameter("@NguoiDungId", NguoiDungId)
                )
                .ToListAsync();
            return result;
        }
        public async Task<string> KiemTraQuyenThietBi(int NguoiDungId, int DeviceId)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<string>("EXEC SP_KiemTraQuyenThietBi @NguoiDungId, @DeviceId",
                    new SqlParameter("@NguoiDungId", NguoiDungId),
                    new SqlParameter("@DeviceId", DeviceId)
                )
                .ToListAsync();
            return result.FirstOrDefault();
        }
        public async Task<Device> LayThongTinThietBi(int DeviceId)
        {
                var result = await _SqlServer.Database
                .SqlQueryRaw<Device>("EXEC SP_LayThongTinThietBi @DeviceId",
                    new SqlParameter("@DeviceId", DeviceId)
                )
                .ToListAsync();
            return result.FirstOrDefault();
        }
        public async Task<string> LayDataThietBi(int DeviceId)
        {
            var data = await _Redis.GetAsync($"device:{DeviceId}:data");
            return data;
        }
        public async Task<string> LayStatusThietBi(int DeviceId)
        {
            var data = await _Redis.GetAsync($"device:{DeviceId}:status");
            return data;
        }
    }
}

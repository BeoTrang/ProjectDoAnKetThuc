using CungCapAPI.Models.Redis;
using CungCapAPI.Models.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public async Task<Name_AX01> LayTenAX01(int deviceid)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<Name_AX01>("EXEC SP_LayTenAX01 @DeviceId",
                    new SqlParameter("@DeviceId", deviceid)
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
        public async Task<string> MaChiaSeThietBi(int DeviceId)
        {
            var data = await _Redis.GetAsync($"device:{DeviceId}:share");
            return data;
        }
        public async Task<bool> XoaMaChiaSeThietBi(int DeviceId)
        {
            try
            {
                await _Redis.RemoveAsync($"device:{DeviceId}:share");
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        public async Task<bool> TaoMaChiaSeThietBi(int deviceid, JObject value, TimeSpan expiry)
        {
            try
            {
                string json = JsonConvert.SerializeObject(value);
                await _Redis.SetAsync($"device:{deviceid}:share", json, expiry);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LuuTenThietBi(LuuTenThietBi model)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<int>("EXEC SP_LuuTenThietBi @DeviceId, @master, @json",
                    new SqlParameter("@DeviceId", model.deviceid),
                    new SqlParameter("@master", model.master),
                    new SqlParameter("@json", model.nameConfig)
                )
                .ToListAsync();
            int KetQua = result.FirstOrDefault();
            if (KetQua == 1)
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

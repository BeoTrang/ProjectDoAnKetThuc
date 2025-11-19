using CungCapAPI.Models.Redis;
using CungCapAPI.Models.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Composition;
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

        public async Task<string> LayMaDangKyThietBi(int userid)
        {
            var data = await _Redis.GetAsync($"user:{userid}:madangkythietbi");
            return data;
        }

        public async Task<int> DangKyThietBiMoi(DangKyThietBi model)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<int>("EXEC SP_DangKyThietBiMoi @NguoiDungId, @DeviceType",
                    new SqlParameter("@NguoiDungId", model.userId),
                    new SqlParameter("@DeviceType", model.deviceType)
                )
                .ToListAsync();
            return result.FirstOrDefault();
        }
        public async Task<bool> SetGiaTriMacDichBanDau(int deviceId, string deviceType)
        {
            JObject status = new JObject
            {
                ["id"] = deviceId.ToString(),
                ["status"] = "0"
            };
            JObject data = new JObject();
            switch (deviceType)
            {
                case "AX01":
                    data = new JObject
                    {
                        ["id"] = deviceId.ToString(),
                        ["type"] = deviceType,
                        ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        ["data"] = new JObject
                        {
                            ["tem"] = 0,
                            ["hum"] = 0
                        },
                        ["relays"] = new JObject
                        {
                            ["relay1"] = 0,
                            ["relay2"] = 0,
                            ["relay3"] = 0,
                            ["relay4"] = 0
                        }
                    };
                    break;
                case "AX02":
                    data = new JObject
                    {
                        ["id"] = deviceId.ToString(),
                        ["type"] = deviceType,
                        ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        ["data"] = new JObject
                        {
                            ["tem"] = 0,
                            ["hum"] = 0
                        }
                    };
                    break;
            }

            string statusString = JsonConvert.SerializeObject(status);
            string dataString = JsonConvert.SerializeObject(data);

            await _Redis.SetAsync($"device:{deviceId}:status", statusString);
            await _Redis.SetAsync($"device:{deviceId}:data", dataString);
            return true;
        }

        public async Task<bool> TaoMaThemThietBi(int userId, JObject value, TimeSpan expiry)
        {
            try
            {
                string json = JsonConvert.SerializeObject(value);
                await _Redis.SetAsync($"user:{userId}:madangkythietbi", json, expiry);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HuyMaThemThietBi(int userId)
        {
            try
            {
                await _Redis.RemoveAsync($"user:{userId}:madangkythietbi");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> DangKyThietBiChiaSe(int userId, int deviceId, string quyen)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<int>("EXEC SP_ThemThietBiChiaSe @NguoiDungId, @DeviceId, @Quyen",
                    new SqlParameter("@NguoiDungId", userId),
                    new SqlParameter("@DeviceId", deviceId),
                    new SqlParameter("@Quyen", quyen)
                )
                .ToListAsync();
            return result.FirstOrDefault();
        }

        

        public async Task<DeviceInfo> LayDeviceInfo(int deviceId)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<DeviceInfo>(
                    "EXEC SP_LayDeviceInfo @DeviceId",
                    new SqlParameter("@DeviceId", deviceId)
                )
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<int> XoaThietBi(int deviceId)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<int>("EXEC SP_XoaThietBi @DeviceId",
                    new SqlParameter("@DeviceId", deviceId)
                )
                .ToListAsync();
            return result.FirstOrDefault();
        }
        public async Task<int> HuyTheoDoiThietBi(int userId, int deviceId)
        {
            var result = await _SqlServer.Database
                .SqlQueryRaw<int>("EXEC SP_HuyTheoDoiThietBi @NguoiDungId, @DeviceId",
                    new SqlParameter("@NguoiDungId", userId),
                    new SqlParameter("@DeviceId", deviceId)
                )
                .ToListAsync();
            return result.FirstOrDefault();
        }
    }
}

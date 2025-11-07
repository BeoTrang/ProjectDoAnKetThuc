using CungCapAPI.Application.Interfaces;
using CungCapAPI.Helpers;
using CungCapAPI.Models.DichVuTrong;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CungCapAPI.Application.Services
{
    public class ThietBiService : IThietBiService
    {
        private readonly ThietBiRepository _thietBiRepository;
        public ThietBiService(ThietBiRepository thietBiRepository)
        {
            _thietBiRepository = thietBiRepository;
        }
        public async Task<string> KiemTraQuyenThietBi(int NguoiDungId, int DeviceId)
        {
            var result = await _thietBiRepository.KiemTraQuyenThietBi(NguoiDungId, DeviceId);
            return result;
        }
        public async Task<JObject> LayDuLieuThietBi(int DeviceId)
        {
            string dataJson = await _thietBiRepository.LayDataThietBi(DeviceId);
            string statusJson = await _thietBiRepository.LayStatusThietBi(DeviceId);
            Device info = await _thietBiRepository.LayThongTinThietBi(DeviceId);
            

            JObject data = string.IsNullOrEmpty(dataJson) ? new JObject() : JObject.Parse(dataJson);
            JObject status = string.IsNullOrEmpty(statusJson) ? new JObject() : JObject.Parse(statusJson);

            data.Merge(status, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Merge
            });

             

            if (info.type == "AX01")
            {
                Name_AX01 name = await _thietBiRepository.LayTenAX01(DeviceId);
                var nameJson = JObject.FromObject(name);
                data["names"] = nameJson;
            }
            
            data["status"] = status["status"];
            

            return data;
        }
        public async Task<List<DanhSachThietBi>> LayDanhSachThietBi(int NguoiDungId)
        {
            List<DanhSachThietBi> result = await _thietBiRepository.DanhSachThietBi(NguoiDungId);
            return result;
        }
        public async Task<Device> LayThongTinThietBi(int DeviceId)
        {
            Device info = await _thietBiRepository.LayThongTinThietBi(DeviceId);
            return info;
        }
        public async Task<bool> TrangThaiThietBi(int DeviceId)
        {
            string statusJson = await _thietBiRepository.LayStatusThietBi(DeviceId);
            JObject status = string.IsNullOrEmpty(statusJson) ? new JObject() : JObject.Parse(statusJson);

            string statusValue = status.Value<string>("status");
            if (statusValue == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Name_AX01> LayTenThietBi(int deviceid)
        {
            Name_AX01 name = await _thietBiRepository.LayTenAX01(deviceid);
            return name;
        }

        public async Task<bool> LuuTenThietBi(LuuTenThietBi model)
        {
            bool KetQua = await _thietBiRepository.LuuTenThietBi(model);
            return KetQua;
        }
        
        public async Task<JObject> MaChiaSeThietBi(int deviceid)
        {
            var KetQua = await _thietBiRepository.MaChiaSeThietBi(deviceid);
            if (KetQua == null)
            {
                return null;
            }
            else
            {
                var data = string.IsNullOrEmpty(KetQua) ? new JObject() : JObject.Parse(KetQua);
                return data;
            }
        }

        public async Task<bool> TaoMaChiaSeThietBi(int deviceid, string quyen)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var data = new byte[10];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            var result = new StringBuilder(10);
            foreach (byte b in data)
            {
                result.Append(chars[b % chars.Length]);
            }

            string Random = deviceid.ToString() + "/" + result.ToString();

            JObject value = new JObject
            {
                ["confirm"] = Random,
                ["quyen"] = quyen
            };

            bool KetQua = await _thietBiRepository.TaoMaChiaSeThietBi(deviceid, value, TimeSpan.FromDays(7));
            return KetQua;
        }

        public async Task<bool> XoaMaChiaSeThietBi(int deviceid)
        {
            return await _thietBiRepository.XoaMaChiaSeThietBi(deviceid);
        }
    }
}

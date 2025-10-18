using CungCapAPI.Application.Interfaces;
using CungCapAPI.Helpers;
using CungCapAPI.Models.DichVuTrong;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            data["status"] = status["status"];
            data["name"] = info.name;

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
    }
}

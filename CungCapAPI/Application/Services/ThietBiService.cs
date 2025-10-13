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
        public async Task<bool> KiemTraQuyenThietBi(int NguoiDungId, int DeviceId)
        {
            var result = await _thietBiRepository.KiemTraQuyenThietBi(NguoiDungId, DeviceId);
            return result;
        }
        public async Task<JObject> LayDuLieuThietBi(int DeviceId)
        {
            string dataJson = await _thietBiRepository.LayDataThietBi(DeviceId);
            string statusJson = await _thietBiRepository.LayStatusThietBi(DeviceId);

            var dataObj = string.IsNullOrEmpty(dataJson) ? new JObject() : JObject.Parse(dataJson);
            var statusObj = string.IsNullOrEmpty(statusJson) ? new JObject() : JObject.Parse(statusJson);

            dataObj.Merge(statusObj, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Merge
            });

            return dataObj;
        }
        public async Task<List<DanhSachThietBi>> LayDanhSachThietBi(int NguoiDungId)
        {
            List<DanhSachThietBi> result = await _thietBiRepository.DanhSachThietBi(NguoiDungId);
            return result;
        }
    }
}

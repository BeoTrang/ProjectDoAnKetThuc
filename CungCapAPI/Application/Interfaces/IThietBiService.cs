using ModelLibrary;
using Newtonsoft.Json.Linq;

namespace CungCapAPI.Application.Interfaces
{
    public interface IThietBiService
    {
        Task<string> KiemTraQuyenThietBi(int NguoiDungId, int DeviceId);
        Task<JObject> LayDuLieuThietBi(int DeviceId);
        Task<List<DanhSachThietBi>> LayDanhSachThietBi(int NguoiDungId);
        Task<Device> LayThongTinThietBi(int DeviceId);
        Task<bool> TrangThaiThietBi(int DeviceId);
    }
}

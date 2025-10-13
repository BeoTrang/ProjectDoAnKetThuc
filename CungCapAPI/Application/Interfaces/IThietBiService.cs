using ModelLibrary;
using Newtonsoft.Json.Linq;

namespace CungCapAPI.Application.Interfaces
{
    public interface IThietBiService
    {
        Task<bool> KiemTraQuyenThietBi(int NguoiDungId, int DeviceId);
        Task<JObject> LayDuLieuThietBi(int DeviceId);
        Task<List<DanhSachThietBi>> LayDanhSachThietBi(int NguoiDungId);
    }
}

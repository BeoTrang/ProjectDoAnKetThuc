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
        Task<Name_AX01> LayTenThietBi(int deviceid);
        Task<bool> LuuTenThietBi(LuuTenThietBi model);
        Task<JObject> MaChiaSeThietBi(int deviceid);
        Task<bool> TaoMaChiaSeThietBi(int deviceid, string quyen);
        Task<bool> XoaMaChiaSeThietBi(int deviceid);
        Task<bool> CheckGhepNoiThietBiVoiTaiKhoan(DangKyThietBi model);
        Task<int> DangKyThietBiMoi(DangKyThietBi model);
        Task<string> LayMaDangKyThietBi(int userId);
        Task<bool> TaoMaThemThietBi(int userId, string deviceType);
        Task<bool> HuyMaThemThietBi(int userId);
        Task<bool> ThemThietBiChiaSe(int userId, ShareDeviceRequest request);
        Task<bool> HuyTheoDoiThietBi(int userId, int deviceId);
        Task<DeviceInfo> LayDeviceInfo(int deviceId);
        Task<bool> XoaThietBi(int deviceId);
    }
}

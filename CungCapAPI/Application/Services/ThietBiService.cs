using CungCapAPI.Application.Interfaces;
using CungCapAPI.Helpers;
using CungCapAPI.Hubs;
using CungCapAPI.Models.DichVuTrong;
using Microsoft.AspNetCore.SignalR;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CungCapAPI.Application.Services
{
    public class ThietBiService : IThietBiService
    {
        private readonly ThietBiRepository _thietBiRepository;
        private readonly IHubContext<DeviceHub> _hubContext;
        public ThietBiService(ThietBiRepository thietBiRepository, IHubContext<DeviceHub> hubContext)
        {
            _thietBiRepository = thietBiRepository;
            _hubContext = hubContext;
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

            switch (info.type)
            {
                case "AX01":
                    Name_AX01 name_AX01 = await _thietBiRepository.LayTenAX01(DeviceId);
                    var nameJson_AX01 = JObject.FromObject(name_AX01);
                    data["names"] = nameJson_AX01;
                    break;

                case "AX02":
                    Name_AX02 name_AX02 = await _thietBiRepository.LayTenAX02(DeviceId);
                    var nameJson_AX02 = JObject.FromObject(name_AX02);
                    data["names"] = nameJson_AX02;
                    break;
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

        public async Task<Name_AX01> LayTenThietBi_AX01(int deviceid)
        {
            Name_AX01 name = await _thietBiRepository.LayTenAX01(deviceid);
            return name;
        }

        public async Task<Name_AX02> LayTenThietBi_AX02(int deviceid)
        {
            Name_AX02 name = await _thietBiRepository.LayTenAX02(deviceid);
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
            var data = new byte[6];
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

        public async Task<bool> CheckGhepNoiThietBiVoiTaiKhoan(DangKyThietBi model)
        {
            try
            {
                string data = await _thietBiRepository.LayMaDangKyThietBi(model.userId);
                JObject dataJson = string.IsNullOrEmpty(data) ? new JObject() : JObject.Parse(data);
                if (dataJson == null)
                {
                    return false;
                }
                else
                {
                    int userId = Convert.ToInt32(dataJson["userId"]);
                    string deviceType = dataJson["deviceType"].ToString();
                    string userToken = dataJson["userToken"].ToString();

                    if (model.userId == userId && model.userToken == userToken && model.deviceType == deviceType)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> DangKyThietBiMoi(DangKyThietBi model)
        {
            int deviceId = await _thietBiRepository.DangKyThietBiMoi(model);
            string groupName = "ThongBao_" + model.userId;
            string payloadString;
            
            if (deviceId == 0)
            {
                var payloadObject = new
                {
                    success = false,
                    message = "Thêm thiết bị thất bại"
                };
                payloadString = JsonConvert.SerializeObject(payloadObject);
            }
            else
            {
                bool IDontKnow = await _thietBiRepository.SetGiaTriMacDichBanDau(deviceId, model.deviceType);
                var payloadObject = new
                {
                    success = true,
                    message = "Thêm thiết bị thành công"
                };
                payloadString = JsonConvert.SerializeObject(payloadObject);
            }
            await _hubContext.Clients.Group(groupName).SendAsync("GuiThongBao", payloadString);
            return deviceId;
        }

        public async Task<string> LayMaDangKyThietBi(int userId)
        {
            string data = await _thietBiRepository.LayMaDangKyThietBi(userId);
            JObject dataJson = string.IsNullOrEmpty(data) ? new JObject() : JObject.Parse(data);

            var userToken = dataJson["userToken"]?.ToString();

            if (string.IsNullOrEmpty(userToken))
            {
                return null;
            }

            return userToken;
        }

        public async Task<bool> TaoMaThemThietBi(int userId, string deviceType)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var data = new byte[6];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            var result = new StringBuilder(10);
            foreach (byte b in data)
            {
                result.Append(chars[b % chars.Length]);
            }

            string Random = result.ToString();

            JObject value = new JObject
            {
                ["userId"] = userId,
                ["userToken"] = Random,
                ["deviceType"] = deviceType
            };

            bool KetQua = await _thietBiRepository.TaoMaThemThietBi(userId, value, TimeSpan.FromDays(1));
            return KetQua;
        }

        public async Task<bool> HuyMaThemThietBi(int userId)
        {
            return await _thietBiRepository.HuyMaThemThietBi(userId);
        }

        public async Task<bool> ThemThietBiChiaSe(int userId, ShareDeviceRequest request)
        {
            try
            {
                string[] Ma = (request.maThietBi).Split('/');
                int DeviceId = int.Parse(Ma[0]);
                string data = await _thietBiRepository.MaChiaSeThietBi(DeviceId);
                
                
                if (request.maThietBi == null || data == null)
                {
                    return false;
                }
                else
                {
                    JObject dataJson = string.IsNullOrEmpty(data) ? new JObject() : JObject.Parse(data);
                    string quyen = dataJson["quyen"]?.ToString();
                    string maXacNhan = dataJson["confirm"]?.ToString();

                    if (request.maThietBi == maXacNhan)
                    {
                        int KetQua = await _thietBiRepository.DangKyThietBiChiaSe(userId, DeviceId, quyen);
                        if (KetQua != 1)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> HuyTheoDoiThietBi(int userId, int deviceId)
        {
            try
            {
                int KetQua = await _thietBiRepository.HuyTheoDoiThietBi(userId, deviceId);
                if (KetQua != 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> XoaThietBi(int deviceId)
        {

            try
            {
                int KetQua = await _thietBiRepository.XoaThietBi(deviceId);
                if (KetQua != 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<DeviceInfo> LayDeviceInfo(int deviceId)
        {
            try
            {
                var KetQua = await _thietBiRepository.LayDeviceInfo(deviceId);
                return KetQua;
            }
            catch
            {
                return null;
            }
        }
        
    }
}

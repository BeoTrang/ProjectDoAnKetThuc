using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public class KiemTraQuyenThietBi
    {
        public int deviceId { get; set; }
    }

    public class ThemThietBi
    {
        public int userId { get; set; }
        public string maThemThietBi { get; set; }
    }
    public class KieuThietBi
    {
        public string deviceType { get; set; }
    }
    public class DangKyThietBi
    {
        public int userId { get; set; }
        public string deviceType { get; set; }
        public string userToken { get; set; }
    }
    public class ShareRequest123
    {
        public int deviceid { get; set; }
    }
    public class DieuKhienThietBi
    {
        public int deviceId { get; set; }
        public string control { get; set; }
        public string state { get; set; }
    }
    public class ShareDeviceModel
    {
        public string confirm { get; set; } = null;
        public string quyen { get; set; } = null;
    }

    public class ShareDeviceRequest
    {
        public string maThietBi { get; set; }
    }
    public class Device
    {
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string status { get; set; }
    }
    public class Devices
    {
        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string firmware { get; set; }
        public DateTime timestamp { get; set; }
    }
    public class AX01 <TData, TRelay, TName> : Devices
    {
        public TData data { get; set; }
        public TRelay relays { get; set; }
        public TName names { get; set; }
    }

    public class Name_AX01  
    {
        public int id { get; set; }
        public string type { get; set; }
        public string master { get; set; }
        public string relay1 { get; set; }
        public string relay2 { get; set; }
        public string relay3 { get; set; }
        public string relay4 { get; set; }
    }


    public class AX02<TData, TName> : Devices
    {
        public TData data { get; set; }
        public TName names { get; set; }
    }

    public class Name_AX02
    {
        public int id { get; set; }
        public string type { get; set; }
        public string master { get; set; }

    }

    public class DHT22
    {
        public double tem { get; set; }
        public double hum { get; set; }
    }
    public class Relay4
    {
        public int relay1 { get; set; }
        public int relay2 { get; set; }
        public int relay3 { get; set; }
        public int relay4 { get; set; }

    }

    public class DanhSachThietBi
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
    }

    public class LayViewThietBi
    {
        public int deviceId { get; set; }
        public string deviceType { get; set; }
    }
    public class LuuTenThietBi
    {
        public int deviceid { get; set; }
        public string master { get; set; }
        public string nameConfig { get; set; }
    }

    public class RequestJObject
    {
        public Object data { get; set; }
    }

    public class EspResponse
    {
        public int userId { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
    }

    public class HistorySearch  
    {
        public int deviceId { get; set; }
        public string typePick { get; set; } = "";
        public string type { get; set; } = "";
        public string pickTime { get; set; } = "";
        public DateTime startUTC { get; set; }
        public DateTime endUTC { get; set; }
    }

    public class DeviceInfo
    {
        public int deviceId { get; set; }
        public string deviceType { get; set; }
        public DateTime deviceTime { get; set; }
    }

    public class Nguong_AX01
    {
        public int deviceId { get; set; }
        public string deviceType { get; set; }
        public double temNguongTren { get; set; }
        public double temNguongDuoi { get; set; }
        public double humNguongTren { get; set; }
        public double humNguongDuoi { get; set; }

        public bool temIsAlert { get; set; } = true;
        public bool humIsAlert { get; set; } = true;
        public bool temThongBao { get; set; }
        public bool humThongBao { get; set; }
    }
    public class Nguong_AX02
    {
        public int deviceId { get; set; }
        public string? deviceType { get; set; }
        public double temNguongTren { get; set; }
        public double temNguongDuoi { get; set; }
        public double humNguongTren { get; set; }
        public double humNguongDuoi { get; set; }

        public bool temIsAlert { get; set; } = true;
        public bool humIsAlert { get; set; } = true;

        public bool temThongBao { get; set; }
        public bool humThongBao { get; set; }
    }
}

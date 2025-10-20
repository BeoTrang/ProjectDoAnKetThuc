using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public class KiemTraQuyenThietBi
    {
        public int deviceId { get; set; }
    }

    public class DieuKhienThietBi
    {
        public int deviceId { get; set; }
        public string payload { get; set; }
        public int state { get; set; }
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
        public string type { get; set; }
        public string master { get; set; }
        public string relay1 { get; set; }
        public string relay2 { get; set; }
        public string relay3 { get; set; }
        public string relay4 { get; set; }
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
}

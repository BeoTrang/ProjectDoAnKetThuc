using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public class KiemTraQuyenThietBi
    {
        public int deviceId { get; set; }
    }
    public class Devices
    {
        public string id { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public DateTime timestamp { get; set; }
    }
    public class AX01 <TData, TRelay> : Devices
    {
        public TData data { get; set; }
        public TRelay relays { get; set; }
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


}

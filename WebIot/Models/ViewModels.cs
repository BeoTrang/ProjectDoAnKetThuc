using ModelLibrary;
namespace WebIot.Models
{
    public class TrangChu
    {
        public ThongTinNguoiDung thongTinNguoiDung { get; set; }
    }
    public class ShareDevice
    {
        public int deivceId { get; set; }
        public ShareDeviceModel share { get; set; }
    }
}

using CungCapAPI.Models.DichVuNgoai;
using CungCapAPI.Models.DichVuTrong;
using CungCapAPI.Models.Redis;
using Microsoft.IdentityModel.Tokens;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CungCapAPI.Models.Alert
{
    public class AlertService
    {
        private readonly IRedisService _Redis;
        private readonly Telegram _telegram;
        public AlertService(IRedisService Redis, Telegram telegram)
        {
            _Redis = Redis;
            _telegram = telegram;
        }

        public async Task AlertProcess(int deviceId, string deviceType, string payload)
        {
            try
            {
                switch (deviceType)
                {
                    case "AX01":
                        await KiemTraNguong_AX01(deviceId, payload);
                        break;

                    case "AX02":
                        await KiemTraNguong_AX01(deviceId, payload);
                        break;

                    default:
                        Console.WriteLine("Không biết làm gì");
                        break;
                }
            }
            catch
            {
                Console.WriteLine("Lỗi gì đó ở phần cảnh báo");
            }
        }

        public async Task KiemTraNguong_AX01(int deviceId, string payload)
        {
            var key = $"device:{deviceId}:CanhBao";
            var jsonString = await _Redis.GetAsync(key);

            var nguong = JsonConvert.DeserializeObject<Nguong_AX01>(jsonString);
            if (nguong == null) return;

            var data = JsonConvert.DeserializeObject<AX01<DHT22, Relay4, Name_AX01>>(payload);

            double nhietDo = data.data.tem;
            double doAm = data.data.hum;

            bool temTrongNguong =
                nhietDo > nguong.temNguongDuoi &&
                nhietDo < nguong.temNguongTren;

            bool humTrongNguong =
                doAm > nguong.humNguongDuoi &&
                doAm < nguong.humNguongTren;

            if (!nguong.temIsAlert && !temTrongNguong)
            {
                nguong.temIsAlert = true;

                string message =
                    $"Cảnh báo thiết bị\n" +
                    $"ID: {deviceId}\n" +
                    $"Nhiệt độ: {nhietDo}°C\n" +
                    $"Thông báo: Nhiệt độ vượt ngưỡng an toàn";

                await _telegram.GuiCanhBaoThietBi(deviceId, message);
            }
            else if (nguong.temIsAlert && temTrongNguong)
            {
                nguong.temIsAlert = false;
            }

            if (!nguong.humIsAlert && !humTrongNguong)
            {
                nguong.humIsAlert = true;

                string message =
                    $"Cảnh báo thiết bị\n" +
                    $"ID: {deviceId}\n" +
                    $"Độ ẩm: {doAm}%\n" +
                    $"Thông báo: Độ ẩm vượt ngưỡng an toàn";

                await _telegram.GuiCanhBaoThietBi(deviceId, message);
            }
            else if (nguong.humIsAlert && humTrongNguong)
            {
                nguong.humIsAlert = false;
            }

            await _Redis.SetAsync(key, JsonConvert.SerializeObject(nguong));
        }

        public async Task KiemTraNguong_AX02(int deviceId, string payload)
        {
            var key = $"device:{deviceId}:CanhBao";
            var jsonString = await _Redis.GetAsync(key);

            var nguong = JsonConvert.DeserializeObject<Nguong_AX01>(jsonString);
            if (nguong == null) return;

            var data = JsonConvert.DeserializeObject<AX02<DHT22, Name_AX02>>(payload);

            double nhietDo = data.data.tem;
            double doAm = data.data.hum;

            bool temTrongNguong =
                nhietDo > nguong.temNguongDuoi &&
                nhietDo < nguong.temNguongTren;

            bool humTrongNguong =
                doAm > nguong.humNguongDuoi &&
                doAm < nguong.humNguongTren;

            if (!nguong.temIsAlert && !temTrongNguong)
            {
                nguong.temIsAlert = true;

                string message =
                    $"Cảnh báo thiết bị\n" +
                    $"ID: {deviceId}\n" +
                    $"Nhiệt độ: {nhietDo}°C\n" +
                    $"Thông báo: Nhiệt độ vượt ngưỡng an toàn";

                await _telegram.GuiCanhBaoThietBi(deviceId, message);
            }
            else if (nguong.temIsAlert && temTrongNguong)
            {
                nguong.temIsAlert = false;
            }

            if (!nguong.humIsAlert && !humTrongNguong)
            {
                nguong.humIsAlert = true;

                string message =
                    $"Cảnh báo thiết bị\n" +
                    $"ID: {deviceId}\n" +
                    $"Độ ẩm: {doAm}%\n" +
                    $"Thông báo: Độ ẩm vượt ngưỡng an toàn";

                await _telegram.GuiCanhBaoThietBi(deviceId, message);
            }
            else if (nguong.humIsAlert && humTrongNguong)
            {
                nguong.humIsAlert = false;
            }

            await _Redis.SetAsync(key, JsonConvert.SerializeObject(nguong));
        }
    }
}

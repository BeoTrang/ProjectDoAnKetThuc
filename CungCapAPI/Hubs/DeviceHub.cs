using CungCapAPI.Models.DichVuTrong;
using InfluxDB.Client.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CungCapAPI.Hubs
{
    [Authorize]
    public class DeviceHub : Hub
    {
        private readonly ThietBiRepository _thietBiRepository;
        public DeviceHub(ThietBiRepository thietBiRepository)
        {
            _thietBiRepository = thietBiRepository;
        }
        public async Task ThamGiaGroup(string groupName)
        {
            
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("JoinedGroup", groupName);
        }
        public async Task JoinGroup()
        {
            var nguoiDungIdClaim = Context.User?.FindFirst("NguoiDungId");
            if (nguoiDungIdClaim == null)
            {
                Console.WriteLine("⚠️ Không tìm thấy claim NguoiDungId trong token!");
                await Clients.Caller.SendAsync("JoinGroupError", "Token không hợp lệ hoặc chưa đăng nhập");
                return;
            }

            int NguoiDungId = int.Parse(nguoiDungIdClaim.Value);
            var DanhSachThietBi = await _thietBiRepository.DanhSachThietBi(NguoiDungId);
            Console.WriteLine(DanhSachThietBi);
            foreach (var item in DanhSachThietBi)
            {
                string groupName = "DeviceId_" + item.DeviceId;
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }

            await Clients.Caller.SendAsync("JoinedGroup");
        }

        public async Task RoiGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("LeftGroup", groupName);
        }
        public async Task GuiData(string groupName, string payload)
        {
            await Clients.Group(groupName).SendAsync("SendData", groupName, payload);
        }


        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client {Context.ConnectionId} connected");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client {Context.ConnectionId} disconnected");
            await base.OnDisconnectedAsync(exception);
        }
    }
}

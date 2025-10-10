using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
namespace CungCapAPI.Hubs
{
    [Authorize]
    public class DeviceHub : Hub
    {
        public async Task ThamGiaGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("JoinedGroup", groupName);
        }
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("JoinedGroup", groupName);
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

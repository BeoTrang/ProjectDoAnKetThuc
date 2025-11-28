using CungCapAPI.Models.Redis;
using CungCapAPI.Models.SqlServer;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelLibrary;
using System.Linq;
using System.Net.Http;
namespace CungCapAPI.Models.DichVuNgoai
{
    public class Telegram
    {
        private readonly ApplicationDbContext _SqlServer;
        private readonly IRedisService _Redis;
        private readonly HttpClient _httpClient;
        public Telegram(ApplicationDbContext SqlServer, IRedisService Redis, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _SqlServer = SqlServer;
            _Redis = Redis;
        }

        public async Task GuiThongBaoTelegram(string chat_Id, string bot_Id, string message)
        {
            var url = $"https://api.telegram.org/bot{bot_Id}/sendMessage";
            var data = new Dictionary<string, string>
            {
                { "chat_id", chat_Id },
                { "text", message }
            };

            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(data));
        }
        
    }
}

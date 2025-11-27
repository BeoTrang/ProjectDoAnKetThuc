using CungCapAPI.Models.Redis;
using CungCapAPI.Models.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelLibrary;
using Newtonsoft.Json;
using System.Linq;
namespace CungCapAPI.Models.Cache
{
    public class Cache
    {
        private readonly ApplicationDbContext _SqlServer;
        private readonly IRedisService _Redis;

        public Cache(ApplicationDbContext SqlServer, IRedisService Redis)
        {
            _SqlServer = SqlServer;
            _Redis = Redis;
        }

        public async Task<TelegramInfo> GetTelegramInfo(int userId)
        {
            TelegramInfo Cache = new TelegramInfo()
            {
                tele_bot_id = "",
                tele_chat_id = ""
            };

            string cacheKey = $"user:{userId}:telegram";
            string cache = await _Redis.GetAsync(cacheKey);
            if (cache.IsNullOrEmpty())
            {
                var data = await _SqlServer.Database
                    .SqlQueryRaw<TelegramInfo>("EXEC SP_LayThongTinTelegram @NguoiDungId",
                        new SqlParameter("@NguoiDungId", userId)
                    )
                .ToListAsync();
                Cache = data.FirstOrDefault();

                //Cập nhật Cache
                string json = JsonConvert.SerializeObject(Cache);
                await _Redis.SetAsync(cacheKey, json);
            }
            else
            {
                Cache = JsonConvert.DeserializeObject<TelegramInfo>(cache);
            }
            return Cache;
        }

    }
}

using StackExchange.Redis;

namespace CungCapAPI.Models.Redis
{
    public interface IRedisService
    {
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string> GetAsync(string key);
        Task RemoveAsync(string key);
    }

    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = _redis.GetDatabase();
        }

        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            if (expiry.HasValue)
                await _db.StringSetAsync(key, value, expiry);
            else
                await _db.StringSetAsync(key, value);
        }

        public async Task<string> GetAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }

}

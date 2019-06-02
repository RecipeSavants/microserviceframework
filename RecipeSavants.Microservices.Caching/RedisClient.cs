using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RecipeSavants.Microservices.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Threading.Tasks;

namespace RecipeSavants.Microservices.Caching
{
    public class RedisClient<T> where T: class
    {
        private IRedisCacheClient redis;
        private int cacheLife;
        private CacheType cacheType;
        private string key;
        private LoggingClient logger;

        public RedisClient(LoggingClient _logger, IRedisCacheClient _redis, int _cacheLife, CacheType _cacheType, string _key)
        {
            redis = _redis;
            cacheLife = _cacheLife;
            key = _key;
            logger = _logger;
            cacheType = _cacheType;
        }

        public T Fetch(string Key)
        {
            var t = redis.Db0.Get<T>(key);
            if (t != null)
            {
                logger.LogDebug($"Value fetched from Redis cache - key {Key}");
            }
            if(CacheType.Sliding == cacheType)
            {
                redis.Db0.Add<T>(key, t, TimeSpan.FromMinutes(this.cacheLife));
            }
            return t;
        }


        public async Task DeleteKey(string key)
        {
            try
            {
                await redis.Db0.RemoveAsync(key);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public void Post(T value, string key)
        {
            if (cacheLife == 0)
            {
                cacheLife = (int)Utilities.MinutesToMidnight();
            }
            var t = "";
            t = "Redis";
            switch (cacheType)
            {
                case CacheType.Absolute:
                    redis.Db0.Add<T>(key, value, TimeSpan.FromMinutes(this.cacheLife));
                    break;
                case CacheType.Sliding:
                    redis.Db0.Add<T>(key, value, TimeSpan.FromMinutes(this.cacheLife));
                    break;
            }
            logger.LogDebug($"Value updated in {t} cache - key {key} at {DateTime.Now}");
        }
    }
}

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RecipeSavants.Microservices.Logging;
using System;
using System.Threading.Tasks;

namespace RecipeSavants.Microservices.Caching
{
    public class RedisClient
    {
        private IDistributedCache redis;
        private int cacheLife;
        private CacheType cacheType;
        private string key;
        private LoggingClient logger;

        public RedisClient(LoggingClient _logger, IDistributedCache _redis, int _cacheLife, CacheType _cacheType, string _key)
        {
            redis = _redis;
            cacheLife = _cacheLife;
            key = _key;
            logger = _logger;
            cacheType = _cacheType;
        }

        public string Fetch(string Key)
        {
            var t = redis.GetString(Key);
            if (t != null)
            {
                logger.LogDebug($"Value fetched from Redis cache - key {Key}");
            }
            return t;
        }


        public async Task DeleteKey(string Key)
        {
            try
            {
                await redis.RemoveAsync(Key);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public void Post(string value, string Key)
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
                    redis.SetString(Key, value, new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(cacheLife)
                    });
                    break;
                case CacheType.Sliding:
                    redis.SetString(Key, value, new DistributedCacheEntryOptions()
                    {
                        SlidingExpiration = new TimeSpan(0, 0, cacheLife, 0),
                        AbsoluteExpiration = DateTime.Now.AddMinutes((int)Utilities.MinutesToMidnight())
                    });
                    break;
            }
            logger.LogDebug($"Value updated in {t} cache - key {Key} at {DateTime.Now}");
        }
    }
}

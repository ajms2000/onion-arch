using Microsoft.Extensions.Caching.Distributed;
using MOR.App.Services.Cache;
using System.Collections.Concurrent;

namespace MOR.App.Services.Common.Cache
{
    public class SmartCachedDataProvider : ICachedDataProvider
    {
        private const string CACHE_KEYS_SEP = "|";
        private const string CACHE_KEYS_KEY = "CacheKeys";

        private readonly IDistributedCache Cache;
        private readonly ConcurrentDictionary<string, object> FastCache = new ConcurrentDictionary<string, object>();
        private readonly SemaphoreSlim Synchronizer = new SemaphoreSlim(1);


        public SmartCachedDataProvider(IDistributedCache cache)
        {
            Cache = cache;
        }


        public CacheType Type => CacheType.Distributed;


        public bool ContainsKey(string key)
        {
            var ret = ContainsKeyAsync(key).GetAwaitResult();
            return ret;
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            try
            {
                await Synchronizer.WaitAsync();

                var keys = await GetCacheKeysAsync();

                var ret = keys.Contains(key);
                return ret;
            }
            finally
            {
                Synchronizer.Release();
            }
        }


        public async Task<T> GetOrSetObjectAsync<T>(string key, Func<Task<T>> dataProvider)
        {
            try
            {
                var cacheKeys = await GetCacheKeysAsync();

                var keyExists = cacheKeys.Contains(key);
                var fcKeyExists = FastCache.ContainsKey(key);

                var data = default(T);

                if (!keyExists)
                {
                    data = await dataProvider();
                    var dataStr = data.ToJson();

                    await Synchronizer.WaitAsync();

                    await Cache.SetStringAsync(key, dataStr!);
                    await AddNewCacheKeyAsync(key);

                    FastCache.AddOrUpdate(key, data!, (k, v) => data!);
                }
                else if (!fcKeyExists)
                {
                    var dataStr = await Cache.GetStringAsync(key);
                    data = dataStr.FromJson<T>();

                    FastCache.AddOrUpdate(key, data!, (k, v) => data!);
                }
                else
                {
                    data = (T)FastCache[key];
                }

                return data!;
            }
            finally
            {
                Synchronizer.Release();
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await Synchronizer.WaitAsync();

                await RemoveCacheKeyAsync(key);
                await Cache.RemoveAsync(key);
            }
            finally
            {
                Synchronizer.Release();
            }
        }


        private async Task<List<string>> GetCacheKeysAsync()
        {
            var strKeys = await Cache.GetStringAsync(CACHE_KEYS_KEY);

            if (strKeys.NullOrWhiteSpace())
            {
                return new List<string>();
            }
            else
            {
                var ret = strKeys!.Split(CACHE_KEYS_SEP, StringSplitOptions.RemoveEmptyEntries).ToList();
                return ret;
            }
        }

        private async Task AddNewCacheKeyAsync(string key)
        {
            var keys = await GetCacheKeysAsync();

            if (!keys.Contains(key))
            {
                keys.Add(key);
            }

            var data = CombineKeys(keys);

            await Cache.SetStringAsync(CACHE_KEYS_KEY, data);
        }

        private async Task RemoveCacheKeyAsync(string key)
        {
            var keys = await GetCacheKeysAsync();

            if (keys.Contains(key))
            {
                keys.Remove(key);
            }

            var data = CombineKeys(keys);

            await Cache.SetStringAsync(CACHE_KEYS_KEY, data);
        }


        private static string CombineKeys(IEnumerable<string> keys)
        {
            var ret = string.Join(CACHE_KEYS_SEP, keys.Distinct(StringComparer.Ordinal));
            return ret;
        }
    }
}

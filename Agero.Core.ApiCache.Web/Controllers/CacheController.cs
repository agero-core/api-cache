using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Agero.Core.ApiCache.Models;

namespace Agero.Core.ApiCache.Web.Controllers
{
    [RoutePrefix("cache")]
    public class CacheController : ApiController
    {
        private static readonly Dictionary<string, object> _asyncCache = new Dictionary<string, object>();

        private static readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        private const int _cacheIncrementSize = 5;

        private static readonly AsyncCacheManager _asyncCacheManager =
            new AsyncCacheManager(
                    clearCache: () =>  _asyncCache.Clear() ,
                    getCacheDataAsync: async () => await Task.FromResult(_asyncCache)
                );

        private static readonly CacheManager _cacheManager =
            new CacheManager(
                clearCache: () => _cache.Clear(),
                getCacheData:  () => _cache
            );

        private static void GenerateCache(Dictionary<string, object> cache)
        {
            var cacheStartIndex = cache.Count;
            var cacheEndIndex = cacheStartIndex + +_cacheIncrementSize;

            for (var i = cacheStartIndex; i < cacheEndIndex; i++)
                cache.Add("cacheKey"+i, "cacheValue"+i);
        }

        [Route("asyncClear")]
        [HttpPost]
        public async Task<CacheInfo> ClearAsyncCache()
        {
            await _asyncCacheManager.ClearCacheAsync();

            return await _asyncCacheManager.GetCacheInfoAsync();
        }

        [Route("asyncCache")]
        [HttpGet]
        public async Task<CacheInfo> GetAsyncCache()
        {
            return await _asyncCacheManager.GetCacheInfoAsync();
        }

        [Route("asyncCreate")]
        [HttpPost]
        public async Task<CacheInfo> CreateAsyncCache()
        {
            GenerateCache(_asyncCache);

            return await _asyncCacheManager.GetCacheInfoAsync();
        }

        [Route("clear")]
        [HttpPost]
        public CacheInfo ClearCache()
        {
             _cacheManager.ClearCache();

            return _cacheManager.GetCacheInfo();
        }

        [Route("")]
        [HttpGet]
        public async Task<CacheInfo> GetCache()
        {
            return await _asyncCacheManager.GetCacheInfoAsync();
        }

        [Route("create")]
        [HttpPost]
        public CacheInfo CreateCache()
        {
            GenerateCache(_cache);

            return  _cacheManager.GetCacheInfo();
        }

    }
}

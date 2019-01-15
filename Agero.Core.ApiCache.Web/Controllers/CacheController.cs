using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Agero.Core.ApiCache.Web.Controllers
{
    [RoutePrefix("cache")]
    public class CacheController : ApiController
    {
        private static readonly Dictionary<string, object> _cacheData = new Dictionary<string, object>();

        private static readonly AsyncCacheManager _asyncCacheManager =
            new AsyncCacheManager(clearCache: () => { _cacheData.Clear(); },
                getCacheDataAsync: async () => await Task.FromResult(_cacheData));

        private static void GenerateCache(int cacheSize)
        {
            for(int i = 0; i < cacheSize; i++)
                _cacheData.Add("cacheKey"+i, "cacheValue"+i);
        }

        [Route("clear")]
        [HttpGet]
        public async Task ClearCache()
        {
            await _asyncCacheManager.ClearCacheAsync();
        }

        [Route("")]
        [HttpGet]
        public async Task GetCache()
        {
            await _asyncCacheManager.GetCacheInfoAsync();
        }

        [Route("create")]
        [HttpPost]
        public async Task CreateCache()
        {
            GenerateCache(5);

            await _asyncCacheManager.GetCacheInfoAsync();
        }
    }
}

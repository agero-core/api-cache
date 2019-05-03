using Agero.Core.ApiCache.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Web.Http;

namespace Agero.Core.ApiCache.Web.Controllers
{
    [RoutePrefix("cache")]
    public class CacheController : ApiController
    {
        private static readonly ICacheManager _cacheManager = new CacheManager(
            clearCache: () => Cache.Instance.Clear(),
            getCacheData:  () => Cache.Instance,
            logInfo: (message, data) => Debug.WriteLine($"INFO: {message}{Environment.NewLine}{JsonConvert.SerializeObject(data)}"),
            logError: (message, data) => Debug.WriteLine($"ERROR: {message}{Environment.NewLine}{JsonConvert.SerializeObject(data)}"),
            getClearIntervalInHours: () => 1,
            getThreadSleepTimeInMinutes: () => 1);


        [Route("create")]
        [HttpPost]
        public CacheInfo CreateCache()
        {
            Cache.Instance[Guid.NewGuid().ToString("N")] = Guid.NewGuid().ToString("N");

            return _cacheManager.GetCacheInfo();
        }

        [Route("")]
        [HttpGet]
        public CacheInfo GetCache()
        {
            return _cacheManager.GetCacheInfo();
        }

        [Route("clear")]
        [HttpPost]
        public CacheInfo ClearCache()
        {
             _cacheManager.ClearCache();

            return _cacheManager.GetCacheInfo();
        }

        [Route("agent/stop")]
        [HttpPost]
        public CacheInfo StopAgent()
        {
            _cacheManager.Stop();

            return _cacheManager.GetCacheInfo();
        }

        [Route("agent/start")]
        [HttpPost]
        public CacheInfo StartAgent()
        {
            _cacheManager.Start();

            return _cacheManager.GetCacheInfo();
        }
    }
}

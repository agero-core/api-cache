using System;
using Agero.Core.ApiCache.Models;
using Agero.Core.Checker;
using Microsoft.AspNetCore.Mvc;

namespace Agero.Core.ApiCache.Web.Core.Controllers
{
    [Route("cache")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly ICacheManager _cacheManager;

        public CacheController(ICacheManager cacheManager)
        {
            Check.ArgumentIsNull(cacheManager, nameof(cacheManager));

            _cacheManager = cacheManager;
        }

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

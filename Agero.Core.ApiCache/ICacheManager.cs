using Agero.Core.ApiCache.Models;

namespace Agero.Core.ApiCache
{
    /// <summary>Sync cache manager</summary>
    public interface ICacheManager : IBaseCacheManager
    {
        /// <summary>Forces cache clear</summary>
        void ClearCache();

        /// <summary>Returns cache info and data</summary>
        CacheInfo GetCacheInfo();
    }
}

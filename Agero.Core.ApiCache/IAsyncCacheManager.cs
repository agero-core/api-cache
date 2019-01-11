using Agero.Core.ApiCache.Models;
using System.Threading.Tasks;

namespace Agero.Core.ApiCache
{
    /// <summary>Async cache manager</summary>
    public interface IAsyncCacheManager : IBaseCacheManager
    {
        /// <summary>Forces cache clear</summary>
        Task ClearCacheAsync();

        /// <summary>Returns cache info and data</summary>
        Task<CacheInfo> GetCacheInfoAsync();
    }
}

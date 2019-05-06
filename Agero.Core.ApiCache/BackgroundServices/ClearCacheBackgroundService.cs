#if NETCOREAPP2_1

using System.Threading;
using System.Threading.Tasks;
using Agero.Core.Checker;
using Microsoft.Extensions.Hosting;

namespace Agero.Core.ApiCache
{
    /// <summary>Background service for cache processing</summary>
    internal class ClearCacheBackgroundService : BackgroundService
    {
        private readonly BaseCacheManager _cacheManager;

        /// <summary>Constructor</summary>
        /// <param name="cacheManager">API cache manager</param>
        public ClearCacheBackgroundService(BaseCacheManager cacheManager)
        {
            Check.ArgumentIsNull(cacheManager, nameof(cacheManager));
            
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _cacheManager.StartBackgroundTaskAsync(stoppingToken);
        }
    }
}

#endif
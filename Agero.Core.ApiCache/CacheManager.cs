using Agero.Core.ApiCache.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Agero.Core.ApiCache
{
    /// <summary>Sync cache manager</summary>
    public class CacheManager : BaseCacheManager, ICacheManager
    {
        private readonly Action _clearCache;
        private readonly Func<object> _getCacheData;

        private readonly Action<string, object> _logInfo;
        private readonly Action<string, object> _logError;
        private readonly Func<int> _getClearIntervalInHours;
        private readonly Func<int> _getThreadSleepTimeInMinutes;
        
        /// <summary>Constructor</summary>
        /// <param name="clearCache">Method which clears cache (required)</param>
        /// <param name="getCacheData">Method which returns cache data (optional)</param>
        /// <param name="logInfo">Method which creates information log (optional)</param>
        /// <param name="logError">Method which creates error log (optional)</param>
        /// <param name="getClearIntervalInHours">Method which returns agent's clear cache interval in hours (optional)</param>
        /// <param name="getThreadSleepTimeInMinutes">Method which returns agent's sleep time in minutes between attempts for clearing cache (optional)</param>
        public CacheManager(
            Action clearCache, Func<object> getCacheData = null,
            Action<string, object> logInfo = null, Action<string, object> logError = null, 
            Func<int> getClearIntervalInHours = null, Func<int> getThreadSleepTimeInMinutes = null)
        {
            Helpers.Checker.ArgumentIsNull(clearCache, "clearCache");

            _clearCache = clearCache;
            _getCacheData = getCacheData ?? (() => null);

            _logInfo = logInfo ?? ((s, o) => { });
            _logError = logError ?? ((s, o) => { });

            _getClearIntervalInHours = getClearIntervalInHours ?? (() => DEFAULT_CLEAR_INTERVAL_IN_HOURS);
            _getThreadSleepTimeInMinutes = getThreadSleepTimeInMinutes ?? (() => DEFAULT_THREAD_SLEEP_TIME_IN_MINUTES);
        }

        /// <summary>Forces cache clear</summary>
        public void ClearCache()
        {
            ClearCache(true);
        }

        /// <summary>Returns cache info and data</summary>
        public CacheInfo GetCacheInfo()
        {
            return
                new CacheInfo
                (
                    agent: 
                        new CacheAgentInfo
                        (
                            isStarted: IsStarted, 
                            timestamp: Timestamp, 
                            clearIntervalInHours: _getClearIntervalInHours(), 
                            threadSleepTimeInMinutes: _getThreadSleepTimeInMinutes()
                        ),
                    clearTime: ClearTime,
                    data: _getCacheData()
                );
        }

        /// <summary>Runs clear cache agent's logic</summary>
        /// <param name="cancellationToken">Cancellation token</param>
        protected override async Task RunThreadAsync(CancellationToken cancellationToken)
        {
            try
            {
                Timestamp = UtcNow;

                _logInfo("Clear cache thread started.", null);

                await StartPeriodicallyCacheClearAsync(cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                _logInfo("Clear cache thread was canceled.", new { error = ex });
                throw;
            } 
            catch (Exception ex)
            {
                _logError("Clear cache thread failed.", new { error = ex });
                throw;
            }
            finally
            {
                Timestamp = null;

                _logInfo("Clear cache thread stopped.", null);
            }
        }
       
        private async Task StartPeriodicallyCacheClearAsync(CancellationToken cancellationToken)
        {
            while (IsStarted && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ClearCache(false);
                }
                catch
                {
                    continue;
                }
                finally
                {
                    Timestamp = UtcNow;

                    var threadSleepTimeInMinutes = _getThreadSleepTimeInMinutes();
                    Helpers.Checker.Assert(threadSleepTimeInMinutes > 0, "threadSleepTimeInMinutes > 0");

                    await Task.Delay(TimeSpan.FromMinutes(threadSleepTimeInMinutes), cancellationToken);
                }
            }
        }

        private void ClearCache(bool force)
        {
            try
            {
                _logInfo("Clear cache started.", null);

                if (!force)
                {
                    var clearIntervalInHours = _getClearIntervalInHours();
                    Helpers.Checker.Assert(clearIntervalInHours > 0, "clearCacheIntervalInHours > 0");

                    if (SkipCacheClear(clearIntervalInHours))
                    {
                        _logInfo("Clear cache skipped.", null);
                        return;
                    }
                }

                _clearCache();

                _logInfo("Clear cache completed.", null);

                ClearTime = UtcNow;
            }
            catch (Exception ex)
            {
                _logError("Clear cache error.", new { error = ex });
                throw;
            }
        }
    }
}

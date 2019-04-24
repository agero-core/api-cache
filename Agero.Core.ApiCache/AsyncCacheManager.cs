using Agero.Core.ApiCache.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Agero.Core.Checker;

namespace Agero.Core.ApiCache
{
    /// <summary>Async cache manager</summary>
    public class AsyncCacheManager : BaseCacheManager, IAsyncCacheManager
    {
        private readonly Action _clearCache;
        private readonly Func<Task<object>> _getCacheDataAsync;

        private readonly Func<string, object, Task> _logInfoAsync;
        private readonly Func<string, object, Task> _logErrorAsync;
        private readonly Func<Task<int>> _getClearIntervalInHoursAsync;
        private readonly Func<Task<int>> _getThreadSleepTimeInMinutesAsync;

        /// <summary>Constructor</summary>
        /// <param name="clearCache">Method which clears cache (required)</param>
        /// <param name="getCacheDataAsync">Method which returns cache data (optional)</param>
        /// <param name="logInfoAsync">Method which creates information log (optional)</param>
        /// <param name="logErrorAsync">Method which creates error log (optional)</param>
        /// <param name="getClearIntervalInHoursAsync">Method which returns agent's clear cache interval in hours (optional)</param>
        /// <param name="getThreadSleepTimeInMinutesAsync">Method which returns agent's sleep time in minutes between attempts for clearing cache (optional)</param>
        public AsyncCacheManager(
            Action clearCache, Func<Task<object>> getCacheDataAsync = null,
            Func<string, object, Task> logInfoAsync = null, Func<string, object, Task> logErrorAsync = null,
            Func<Task<int>> getClearIntervalInHoursAsync = null, Func<Task<int>> getThreadSleepTimeInMinutesAsync = null)
        {
            Check.ArgumentIsNull(clearCache, "clearCache");

            _clearCache = clearCache;
            _getCacheDataAsync = getCacheDataAsync ?? (async () => await Task.FromResult<object>(null));

            _logInfoAsync = logInfoAsync ?? (async (s, o) => await Task.FromResult(0));
            _logErrorAsync = logErrorAsync ?? (async (s, o) => await Task.FromResult(0));

            _getClearIntervalInHoursAsync = getClearIntervalInHoursAsync ?? (async () => await Task.FromResult(DEFAULT_CLEAR_INTERVAL_IN_HOURS));
            _getThreadSleepTimeInMinutesAsync = getThreadSleepTimeInMinutesAsync ?? (async () => await Task.FromResult(DEFAULT_THREAD_SLEEP_TIME_IN_MINUTES));
        }

        /// <summary>Forces cache clear</summary>
        public async Task ClearCacheAsync()
        {
            await ClearCacheAsync(true);
        }

        /// <summary>Returns cache info and data</summary>
        public async Task<CacheInfo> GetCacheInfoAsync()
        {
            return
                new CacheInfo
                (
                    agent:
                        new CacheAgentInfo
                        (
                            isStarted: IsStarted,
                            timestamp: Timestamp,
                            clearIntervalInHours: await _getClearIntervalInHoursAsync(),
                            threadSleepTimeInMinutes: await _getThreadSleepTimeInMinutesAsync()
                        ),
                    clearTime: ClearTime,
                    data: await _getCacheDataAsync()
                );
        }

        /// <summary>Runs clear cache agent's logic</summary>
        /// <param name="cancellationToken">Cancellation token</param>
        protected override async Task RunThreadAsync(CancellationToken cancellationToken)
        {
            try
            {
                Timestamp = UtcNow;

                await _logInfoAsync("Clear cache thread started.", null);

                await StartPeriodicallyCacheClearAsync(cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                await _logInfoAsync("Clear cache thread was canceled.", new { error = ex });
                throw;
            }
            catch (Exception ex)
            {
                await _logErrorAsync("Clear cache thread failed.", new { error = ex });
                throw;
            }
            finally
            {
                Timestamp = null;

                await _logInfoAsync("Clear cache thread stopped.", null);
            }
        }

        private async Task StartPeriodicallyCacheClearAsync(CancellationToken cancellationToken)
        {
            while (IsStarted && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await ClearCacheAsync(false);
                }
                catch
                {
                    continue;
                }
                finally
                {
                    Timestamp = UtcNow;

                    var threadSleepTimeInMinutes = await _getThreadSleepTimeInMinutesAsync();
                    Check.Assert(threadSleepTimeInMinutes > 0, "threadSleepTimeInMinutes > 0");

                    await Task.Delay(TimeSpan.FromMinutes(threadSleepTimeInMinutes), cancellationToken);
                }
            }
        }

        private async Task ClearCacheAsync(bool force)
        {
            try
            {
                await _logInfoAsync("Clear cache started.", null);

                if (!force)
                {
                    var clearIntervalInHours = await _getClearIntervalInHoursAsync();
                    Check.Assert(clearIntervalInHours > 0, "clearCacheIntervalInHours > 0");

                    if (SkipCacheClear(clearIntervalInHours))
                    {
                        await _logInfoAsync("Clear cache skipped.", null);
                        return;
                    }
                }

                _clearCache();

                await _logInfoAsync("Clear cache completed.", null);

                ClearTime = UtcNow;
            }
            catch (Exception ex)
            {
                await _logErrorAsync("Clear cache error.", new { error = ex });
                throw;
            }
        }
    }
}

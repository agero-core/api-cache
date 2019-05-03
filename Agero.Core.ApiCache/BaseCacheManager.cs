using Agero.Core.Checker;
using System;
using System.Threading;
using System.Threading.Tasks;
#if NET461
using System.Web.Hosting;    
#endif


namespace Agero.Core.ApiCache
{
    /// <summary>Base cache manager</summary>
    public abstract class BaseCacheManager : IBaseCacheManager
    {
        /// <summary>Default agent's clear cache interval in hours</summary>
        public const int DEFAULT_CLEAR_INTERVAL_IN_HOURS = 24;

        /// <summary>Default agent's sleep time in minutes between attempts for clearing cache</summary>
        public const int DEFAULT_THREAD_SLEEP_TIME_IN_MINUTES = 20;

        private readonly object _syncRoot = new object();

        /// <summary>Constructor</summary>
        protected BaseCacheManager() 
        {
            ClearTime = UtcNow;

#if NET461
            Start();
#endif
        }

        /// <summary>Current UTC time</summary>
        protected DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        /// <summary>Clear cache agent's latest timestamp</summary>
        public DateTimeOffset? Timestamp { get; protected set; }

        /// <summary>Latest clear cache time</summary>
        public DateTimeOffset ClearTime { get; protected set; }

        /// <summary>Returns whether clear cache agent is started</summary>
        public bool IsStarted { get; private set; }

        /// <summary>Starts clear cache agent</summary>
        public void Start()
        {
            IsStarted = true;

#if NET461
            RunStoppedThread();
#endif
        }

        /// <summary>Stops clear cache agent</summary>
        public void Stop()
        {
            IsStarted = false;
        }

#if NETCOREAPP2_1
        /// <summary> Starting point of the background cache agent </summary>
        internal async Task StartBackgroundTaskAsync(CancellationToken cancellationToken)
        {
            IsStarted = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (!IsStarted)
                {
                    await Task.Delay(10_000, cancellationToken);
                    continue;
                }

                await RunThreadAsync(cancellationToken);
            }
        }
#endif

#if NET461
        private void RunStoppedThread()
        {
            if (!IsStarted)
                return;

            lock (_syncRoot)
            {
                if (Timestamp.HasValue)
                    return;

                HostingEnvironment.QueueBackgroundWorkItem(async token => await RunThreadAsync(token));
            }
        }
#endif

        /// <summary>Runs clear cache agent's logic</summary>
        /// <param name="cancellationToken">Cancellation token</param>
        protected abstract Task RunThreadAsync(CancellationToken cancellationToken);

        /// <summary>Returns whether cache clear must be skipped because clear cache interval is not elapsed since latest clear</summary>
        /// <param name="clearIntervalInHours">Clear cache interval in hours</param>
        protected bool SkipCacheClear(int clearIntervalInHours)
        {
            Check.Argument(clearIntervalInHours > 0, "learIntervalInHours > 0");

            return ClearTime.AddHours(clearIntervalInHours) > UtcNow;
        }
    }
}

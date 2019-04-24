using System;

namespace Agero.Core.ApiCache
{
    /// <summary>Base cache manager</summary>
    public interface IBaseCacheManager
    {
        /// <summary>Clear cache agent's latest timestamp</summary>
        DateTimeOffset? Timestamp { get; }

        /// <summary>Latest clear cache time</summary>
        DateTimeOffset ClearTime { get; }

        /// <summary>Starts clear cache agent</summary>
        void Start();

        /// <summary>Stops clear cache agent</summary>
        void Stop();

        /// <summary>Returns whether clear cache agent is started</summary>
        bool IsStarted { get; }
    }
}

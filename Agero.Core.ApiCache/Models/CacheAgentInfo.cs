using Agero.Core.Checker;
using System;
using System.Runtime.Serialization;

namespace Agero.Core.ApiCache.Models
{
    /// <summary>Clear cache agent info</summary>
    [DataContract]
    public class CacheAgentInfo
    {
        /// <summary>Constructor</summary>
        public CacheAgentInfo(bool isStarted, DateTimeOffset? timestamp, int clearIntervalInHours, int threadSleepTimeInMinutes)
        {
            Check.Argument(clearIntervalInHours > 0, "clearIntervalInHours > 0");
            Check.Argument(threadSleepTimeInMinutes > 0, "threadSleepTimeInMinutes > 0");

            IsStarted = isStarted;
            Timestamp = timestamp;
            ClearIntervalInHours = clearIntervalInHours;
            ThreadSleepTimeInMinutes = threadSleepTimeInMinutes;
        }

        /// <summary>Returns whether agent is started</summary>
        [DataMember(Name = "isStarted")]
        public bool IsStarted { get; }

        /// <summary>Agent's latest timestamp</summary>
        [DataMember(Name = "timestamp")]
        public DateTimeOffset? Timestamp { get; }

        /// <summary>Agent's clear cache interval in hours</summary>
        [DataMember(Name = "clearIntervalInHours")]
        public int ClearIntervalInHours { get; }

        /// <summary>Agent's sleep time in minutes between attempts for clearing cache</summary>
        [DataMember(Name = "threadSleepTimeInMinutes")]
        public int ThreadSleepTimeInMinutes { get; }
    }
}

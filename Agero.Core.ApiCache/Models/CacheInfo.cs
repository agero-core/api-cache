using Agero.Core.Checker;
using System;
using System.Runtime.Serialization;

namespace Agero.Core.ApiCache.Models
{
    /// <summary>Cache info</summary>
    [DataContract]
    public class CacheInfo
    {
        /// <summary>Constructor</summary>
        public CacheInfo(CacheAgentInfo agent, DateTimeOffset clearTime, object data = null)
        {
            Check.ArgumentIsNull(agent, "agent");

            Agent = agent;
            ClearTime = clearTime;

            Data = data;
        }
        
        /// <summary>Clear cache agent info</summary>
        [DataMember(Name = "agent")]
        public CacheAgentInfo Agent { get; }

        /// <summary>Latest clear cache time</summary>
        [DataMember(Name = "clearTime")]
        public DateTimeOffset ClearTime { get; }

        /// <summary>Cache data</summary>
        [DataMember(Name = "data")]
        public object Data { get; }
    }
}

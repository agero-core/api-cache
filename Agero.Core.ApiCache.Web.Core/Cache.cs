using System.Collections.Generic;

namespace Agero.Core.ApiCache.Web.Core
{
    /// <summary>Test cache</summary>
    public static class Cache
    {
        public static IDictionary<string, object> Instance { get; } = new Dictionary<string, object>();
    }
}

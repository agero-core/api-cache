# Api Cache

API cache manager library for .NET Framework.

The library provides a background api cache agent that periodically checks if the cache needs to be cleared. The Api cache client implements the cache and passes along implementations as functions to **AsyncCacheManager** or **CacheManager**, informing how to clear cache and retrieve cache. Please see below for an usage example of **AsyncCacheManager**. For additional usage information see [Agero.Core.ApiCache.Web](./Agero.Core.ApiCache.Web)

## Usage:
Create instance:
```csharp
//Example cache
IDictionary<string, object> _cache =
    new Dictionary<string, object>()
        {
            {"CacheKey1","CacheVal1"},
            {"CacheKey2","CacheVal2"},
        };

IAsyncCacheManager _asyncCacheManager =
    new AsyncCacheManager(
        //required implementation to handle clearing cache 
        clearCache: () => _cache.Clear(),
        //optional implementation to handle return cache
        getCacheDataAsync: async () => await Task.FromResult(_cache),
        //optional implementation to handle information logging
        logInfoAsync: async (message, obj) => await Task.FromResult(0),
        //optional implementation to handle logging errors
        logErrorAsync: async (message, obj) => await Task.FromResult(0),
        //optional implementation returning the number of hours the cache needs to be cleared
        getClearIntervalInHoursAsync: async () => await Task.FromResult(24),
        //optional implementation returning number of minutes the cache background thread 
        //needs to sleep before attempting to clear cache again.
        getThreadSleepTimeInMinutesAsync: async () => await Task.FromResult(20)     
        );
```

Clear cache:
```csharp
await _asyncCacheManager.ClearCacheAsync();  
```

Get cache:
```csharp
CacheInfo cacheInfo = await _asyncCacheManager.GetCacheInfoAsync();  

var json = JsonConvert.SerializeObject(cacheInfo);  
```

The above code generates the below json.
```json
{  
   "agent":{  
      "isStarted":true,
      "timestamp":"2019-01-15T21:03:52.8622627+00:00",
      "clearIntervalInHours":24,
      "threadSleepTimeInMinutes":20
   },
   "clearTime":"2019-01-15T21:03:52.9128548+00:00",
   "data":{  
      "CacheKey1" : "CacheVal1",
      "CacheKey2" : "CacheVal2"
   }
}
```
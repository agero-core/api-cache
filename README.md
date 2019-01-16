# Api Cache

API cache manager library for .NET Framework.

## Usage:
Create instance:
```csharp
IDictionary<string, object> _cache =
    new Dictionary<string, object>()
        {
            {"CacheKey1","CacheVal1"},
            {"CacheKey2","CacheVal2"},
        };

IAsyncCacheManager _asyncCacheManager =
    new AsyncCacheManager(
        //implementation to handle clearing cache
        clearCache: () => _cache.Clear(),
        //implementation to return cache
        getCacheDataAsync: async () => await Task.FromResult(_cache),
        //implementation handle log
        logInfoAsync: async (message, obj) => await Task.FromResult(0),
        //implementation to handle errors
        logErrorAsync: async (message, obj) => await Task.FromResult(0),
        //implementation returning number of hours the cache needs to be cleared
        getClearIntervalInHoursAsync: async () => await Task.FromResult(24),
        //implementation returning number of minutes the cache background thread 
        //needs to sleep before attempting to clear cache again.
        getThreadSleepTimeInMinutesAsync: async () => await Task.FromResult(20)     
        );
```

Clear Cache:
```csharp
await _asyncCacheManager.ClearCacheAsync();  
```

Get Cache:
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
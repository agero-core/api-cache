# api-cache

API cache manager library for .NET Framework.

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
            clearCache: () => {
                //implementation to handle clearing cache
                _cache.Clear()
            },
            getCacheDataAsync: async () => 
            {
                //implementation to return cache
                await Task.FromResult(_cache);
            },
            logInfoAsync: async (message, object) => {
               //implementation of how to handle log
            }, 
            logErrorAsync: async (message, object) =>{
                //implementation of how to handle errors
            },
            getClearIntervalInHoursAsync: async () => {
                //implementation returning number of hours the cache needs to be cleared
                await Task.FromResult(24);
            }
            getThreadSleepTimeInMinutesAsync: async () => {
                //implementation returning number of minutes the cache background thread 
                //needs to sleep before attempting to clear cache again.
                await Task.FromResult(20)
            }
        );
```


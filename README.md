# API Cache

API Cache is a **.NET Framework (>= v4.6.1)** and **.NET Core (>=2.1)** library for in-memory cache management in ASP.NET applications.

The library provides a background agent that periodically clears in-memory cache.

## Setup
Let's assume we have an in-memory cache:
```csharp
public static class Cache
{
	public static IDictionary<string, object> Instance { get; } = new Dictionary<string, object>();
}
```

Create and setup instance of [ICacheManager](./Agero.Core.ApiCache/ICacheManager.cs) or [IAsyncCacheManager](./Agero.Core.ApiCache/IAsyncCacheManager.cs) in a **static** context:
```csharp
var cacheManager = new CacheManager(
	// Method which clears cache
	clearCache: () => Cache.Instance.Clear(),
	// Method which returns cached data
	getCacheData: () => Cache.Instance,
	// Setup info logging 
	logInfo: (message, data) => Debug.WriteLine($"INFO: {message}{Environment.NewLine}{JsonConvert.SerializeObject(data)}"),
	// Setup error logging 
	logError: (message, data) => Debug.WriteLine($"ERROR: {message}{Environment.NewLine}{JsonConvert.SerializeObject(data)}"),
	// Method which returns interval in hours when cache needs to be cleared again
	getClearIntervalInHours: () => 1,
	// Method which returns background thread's sleep time in minutes before attempting to clear cache again
	getThreadSleepTimeInMinutes: () => 5);
```

Only for .NET Core, add "API Cache" services to dependency injection container using [extension method](./Agero.Core.ApiCache/Extensions/ServiceCollectionExtensions.cs):
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddApiCache(cacheManager);
}
```

## Usage:
Start background agent:
 ```csharp
cacheManager.Start();  
```

Stop background agent:
 ```csharp
cacheManager.Stop();  
```

Force to clear cache:
```csharp
cacheManager.ClearCache();  
```

Get agent status and cached data:
```csharp
Cache.Instance[Guid.NewGuid().ToString("N")] = Guid.NewGuid().ToString("N");

var cacheInfo = cacheManager.GetCacheInfo();  

var json = JsonConvert.SerializeObject(cacheInfo);  
```

The above code generates the below JSON:
```json
{
    "agent": {
        "isStarted": true,
        "timestamp": "2019-04-24T15:17:04.2907437+00:00",
        "clearIntervalInHours": 1,
        "threadSleepTimeInMinutes": 5
    },
    "clearTime": "2019-04-24T15:17:04.233738+00:00",
    "data": {
        "681d8917ff674e0b8dc7a59d08a31bfd": "eb375c11fd1a41b0a5c2c53e754c5c6c"
    }
}
```

For additional usage related info please see [Agero.Core.ApiCache.Web](./Agero.Core.ApiCache.Web/) (.NET Framework) and [Agero.Core.ApiCache.Web.Core](./Agero.Core.ApiCache.Web.Core/) (.NET Core).
#if NETCOREAPP2_1

using Agero.Core.Checker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Agero.Core.ApiCache
{
    /// <summary>Extensions for <see cref="IServiceCollection"/></summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds "API Cache" services to specified <see cref="IServiceCollection"/></summary>
        /// <typeparam name="TCacheManager">Cache manager type</typeparam>
        /// <param name="services">ASP .NET Core dependency injection container</param>
        /// <param name="cacheManager">API cache manager</param>
        public static void AddApiCache<TCacheManager>(this IServiceCollection services, TCacheManager cacheManager)
            where TCacheManager : BaseCacheManager
        {
            Check.ArgumentIsNull(services, nameof(services));
            Check.ArgumentIsNull(cacheManager, nameof(cacheManager));

            if (services.Any(sd => sd.ImplementationType == typeof(ClearCacheBackgroundService)))
                throw new InvalidOperationException($"'{nameof(AddApiCache)}' can be used only once.");

            services.AddTransient<IHostedService, ClearCacheBackgroundService>(_ => new ClearCacheBackgroundService(cacheManager));
        }
    }
}

#endif
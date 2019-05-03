#if NETCOREAPP2_1
using Agero.Core.Checker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace Agero.Core.ApiCache
{
    /// <summary>Extensions for <see cref="IServiceCollection"/></summary>
    public static class ServiceCollectionExtensions
    {
        private static int _executionCounter;

        /// <summary>Adds "API Cache" services to specified <see cref="IServiceCollection"/></summary>
        /// <typeparam name="TCacheManager">Cache manager type</typeparam>
        /// <param name="services">ASP .NET Core dependency injection container</param>
        /// <param name="cacheManager">API cache manager</param>
        public static void AddApiCache<TCacheManager>(this IServiceCollection services, TCacheManager cacheManager)
            where TCacheManager : BaseCacheManager
        {
            Check.ArgumentIsNull(services, nameof(services));
            Check.ArgumentIsNull(cacheManager, nameof(cacheManager));

            Check.Assert(Interlocked.Increment(ref _executionCounter) == 1, $"'{nameof(AddApiCache)}' method can be used only once.");
            
            services.AddTransient<IHostedService>(_ => new ClearCacheBackgroundService(cacheManager));
        }
    }
}

#endif
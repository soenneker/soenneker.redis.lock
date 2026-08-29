using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Redis.Lock.Abstract;
using Soenneker.Redis.Util.Registrars;

namespace Soenneker.Redis.Lock.Registrars;

/// <summary>
/// A utility library leveraging Redis that provides distributed locking
/// </summary>
public static class RedisLockUtilRegistrar
{
    /// <summary>
    /// Registers Redis Lock Util with a singleton lifetime.
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddRedisLockUtilAsSingleton(this IServiceCollection services)
    {
        services.AddRedisUtilAsSingleton()
                .TryAddSingleton<IRedisLockUtil, RedisLockUtil>();

        return services;
    }

    /// <summary>
    /// Registers Redis Lock Util with a scoped lifetime.
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddRedisLockUtilAsScoped(this IServiceCollection services)
    {
        services.AddRedisUtilAsScoped()
                .TryAddScoped<IRedisLockUtil, RedisLockUtil>();

        return services;
    }
}

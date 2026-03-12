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
    public static IServiceCollection AddRedisLockUtilAsSingleton(this IServiceCollection services)
    {
        services.AddRedisUtilAsSingleton()
                .TryAddSingleton<IRedisLockUtil, RedisLockUtil>();

        return services;
    }

    public static IServiceCollection AddRedisLockUtilAsScoped(this IServiceCollection services)
    {
        services.AddRedisUtilAsScoped()
                .TryAddScoped<IRedisLockUtil, RedisLockUtil>();

        return services;
    }
}
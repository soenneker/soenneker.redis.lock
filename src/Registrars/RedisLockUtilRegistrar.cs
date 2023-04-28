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
    public static void AddRedisLockUtilAsSingleton(this IServiceCollection services)
    {
        services.AddRedisUtilAsSingleton();
        services.TryAddSingleton<IRedisLockUtil, RedisLockUtil>();
    }

    public static void AddRedisLockUtilAsScoped(this IServiceCollection services)
    {
        services.AddRedisUtilAsSingleton();
        services.TryAddScoped<IRedisLockUtil, RedisLockUtil>();
    }
}
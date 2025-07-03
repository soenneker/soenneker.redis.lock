using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.ValueTask;
using Soenneker.Redis.Lock.Abstract;
using Soenneker.Redis.Util.Abstract;

namespace Soenneker.Redis.Lock;

/// <inheritdoc cref="IRedisLockUtil"/>
public sealed class RedisLockUtil : IRedisLockUtil
{
    private readonly IRedisUtil _redisUtil;
    private readonly ILogger<RedisLockUtil> _logger;

    public RedisLockUtil(IRedisUtil redisUtil, ILogger<RedisLockUtil> logger)
    {
        _redisUtil = redisUtil;
        _logger = logger;
    }

    public async ValueTask<bool> Check(string lockName, CancellationToken cancellationToken = default)
    {
        string? sourcesLock = await _redisUtil.GetString(lockName, cancellationToken).NoSync();

        bool result = sourcesLock != null;

        if (result)
            _logger.LogDebug("Redis lock ({name}) is currently set", lockName);

        return result;
    }

    public ValueTask Lock(string lockName, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Locking Redis lock ({name})...", lockName);

        return _redisUtil.Set(lockName, "1", cancellationToken: cancellationToken);
    }

    public ValueTask Unlock(string lockName, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Unlocking Redis lock ({name})...", lockName);

        return _redisUtil.Remove(lockName, cancellationToken: cancellationToken);
    }

    public async Task UnlockAll(IEnumerable<string> locks, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Unlocking all Redis locks...");

        foreach (string lockName in locks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Unlock(lockName, cancellationToken).NoSync();
        }

        _logger.LogDebug("All Redis locks have been removed");
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.ValueTask;
using Soenneker.Redis.Lock.Abstract;
using Soenneker.Redis.Util.Abstract;

namespace Soenneker.Redis.Lock;

/// <inheritdoc cref="IRedisLockUtil" />
public sealed class RedisLockUtil : IRedisLockUtil
{
    private const string _lockValue = "1";

    private readonly IRedisUtil _redisUtil;
    private readonly ILogger<RedisLockUtil> _logger;

    public RedisLockUtil(IRedisUtil redisUtil, ILogger<RedisLockUtil> logger)
    {
        _redisUtil = redisUtil;
        _logger = logger;
    }

    public async ValueTask<bool> Check(string lockName, CancellationToken cancellationToken = default)
    {
        string? value = await _redisUtil.GetString(lockName, cancellationToken).NoSync();

        bool result = value is not null;

        if (result)
            _logger.LogDebug("Redis lock ({lockName}) is currently set", lockName);

        return result;
    }

    public async ValueTask<RedisLockHandle?> TryLock(string lockName, System.TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        ValidateExpiration(expiration);

        _logger.LogDebug("Attempting to set Redis lock ({lockName}) with expiration ({expiration})...", lockName, expiration);

        var lockToken = System.Guid.NewGuid().ToString();
        bool acquired = await _redisUtil.SetIfNotExists(lockName, lockToken, expiration, cancellationToken).NoSync();

        return acquired ? new RedisLockHandle(this, lockName, lockToken) : null;
    }

    public ValueTask Lock(string lockName, System.TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        ValidateExpiration(expiration);

        _logger.LogDebug("Setting Redis lock ({lockName}) with expiration ({expiration})...", lockName, expiration);

        return _redisUtil.Set(lockName, _lockValue, expiration, cancellationToken: cancellationToken);
    }

    public ValueTask<bool> Unlock(string lockName, CancellationToken cancellationToken = default)
    {
        return Unlock(lockName, _lockValue, cancellationToken);
    }

    public ValueTask<bool> Unlock(string lockName, string lockValue, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Unlocking Redis lock ({lockName}) if its value matches...", lockName);

        return _redisUtil.RemoveIfEqual(lockName, lockValue, cancellationToken);
    }

    public ValueTask ForceUnlock(string lockName, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Forcibly unlocking Redis lock ({lockName}) without checking its value...", lockName);

        return _redisUtil.Remove(lockName, cancellationToken: cancellationToken);
    }

    public async Task ForceUnlockAll(IEnumerable<string> locks, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Forcibly unlocking all Redis locks...");

        foreach (string lockName in locks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await ForceUnlock(lockName, cancellationToken).NoSync();
        }

        _logger.LogDebug("All Redis locks have been removed");
    }

    private static void ValidateExpiration(System.TimeSpan expiration)
    {
        if (expiration <= System.TimeSpan.Zero)
            throw new System.ArgumentOutOfRangeException(nameof(expiration), "Expiration must be greater than zero.");
    }
}

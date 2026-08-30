using System;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Atomics.ValueBools;
using Soenneker.Extensions.ValueTask;
using Soenneker.Redis.Lock.Abstract;

namespace Soenneker.Redis.Lock;

/// <summary>
/// Represents ownership of a distributed Redis lock acquired through <see cref="IRedisLockUtil.TryLock"/>.
/// </summary>
public sealed class RedisLockHandle : IAsyncDisposable
{
    private readonly RedisLockUtil _redisLockUtil;
    private ValueAtomicBool _disposed;

    /// <summary>Gets the Redis key used for the lock.</summary>
    public string LockName { get; }

    /// <summary>Gets the unique value that proves ownership of the lock.</summary>
    public string LockValue { get; }

    internal RedisLockHandle(RedisLockUtil redisLockUtil, string lockName, string lockToken)
    {
        _redisLockUtil = redisLockUtil;
        LockName = lockName;
        LockValue = lockToken;
    }

    /// <summary>Releases the lock if its current value still matches this handle's ownership token.</summary>
    public async ValueTask DisposeAsync()
    {
        if (!_disposed.TrySetTrue())
            return;

        _ = await _redisLockUtil.Unlock(LockName, LockValue, CancellationToken.None).NoSync();
    }
}

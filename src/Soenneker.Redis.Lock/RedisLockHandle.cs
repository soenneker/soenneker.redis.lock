using System;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Atomics.ValueBools;
using Soenneker.Extensions.ValueTask;

namespace Soenneker.Redis.Lock;

/// <summary>
/// Represents ownership of a distributed Redis lock acquired through <see cref="RedisLockUtil.TryLock"/>.
/// </summary>
public sealed class RedisLockHandle : IAsyncDisposable
{
    private readonly RedisLockUtil _redisLockUtil;
    private ValueAtomicBool _disposed;

    public string LockName { get; }
    public string LockValue { get; }

    internal RedisLockHandle(RedisLockUtil redisLockUtil, string lockName, string lockToken)
    {
        _redisLockUtil = redisLockUtil;
        LockName = lockName;
        LockValue = lockToken;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed.TrySetTrue())
            return;

        _ = await _redisLockUtil.Unlock(LockName, LockValue, CancellationToken.None).NoSync();
    }
}

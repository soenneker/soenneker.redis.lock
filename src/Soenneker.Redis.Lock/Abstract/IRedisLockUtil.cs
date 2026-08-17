using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Redis.Lock.Abstract;

/// <summary>
/// A utility library leveraging Redis that provides distributed locking <para/>
/// Typically Scoped IoC
/// </summary>
public interface IRedisLockUtil
{
    /// <summary>
    /// Checks if a Redis lock with the specified name is currently set.
    /// </summary>
    /// <param name="lockName">The name of the lock to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><c>true</c> if the lock is currently set; otherwise <c>false</c>.</returns>
    [Pure]
    ValueTask<bool> Check(string lockName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to acquire a Redis lock only when it is not already set.
    /// </summary>
    /// <param name="lockName">The name of the lock to acquire.</param>
    /// <param name="expiration">The amount of time after which the lock automatically expires.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A handle that owns and releases the lock if acquisition succeeded; otherwise <c>null</c>.</returns>
    ValueTask<RedisLockHandle?> TryLock(string lockName, System.TimeSpan expiration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a Redis lock with the specified name and expiration.
    /// </summary>
    /// <param name="lockName">The name of the lock to set.</param>
    /// <param name="expiration">The amount of time after which the lock automatically expires.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Lock(string lockName, System.TimeSpan expiration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases a Redis lock created by <see cref="Lock"/>.
    /// </summary>
    /// <param name="lockName">The name of the lock to release.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><c>true</c> if the lock was released; otherwise <c>false</c>.</returns>
    ValueTask<bool> Unlock(string lockName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases a Redis lock only when its current value matches the specified lock value.
    /// </summary>
    /// <param name="lockName">The name of the lock to release.</param>
    /// <param name="lockValue">The value that must currently be stored for the lock to be released.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><c>true</c> if the value matched and the lock was released; otherwise <c>false</c>.</returns>
    ValueTask<bool> Unlock(string lockName, string lockValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Forcibly releases a Redis lock without checking its value.
    /// </summary>
    /// <param name="lockName">The name of the lock to release.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask ForceUnlock(string lockName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Forcibly releases multiple Redis locks without checking ownership.
    /// </summary>
    /// <param name="locks">A collection of lock names to release.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ForceUnlockAll(IEnumerable<string> locks, CancellationToken cancellationToken = default);
}

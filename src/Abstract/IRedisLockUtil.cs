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
    /// <returns>A <see cref="ValueTask{Boolean}"/> indicating whether the lock is currently set.</returns>
    [Pure]
    ValueTask<bool> Check(string lockName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Acquires a Redis lock with the specified name.
    /// </summary>
    /// <param name="lockName">The name of the lock to acquire.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Lock(string lockName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases a Redis lock with the specified name.
    /// </summary>
    /// <param name="lockName">The name of the lock to release.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Unlock(string lockName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases multiple Redis locks based on the provided list of lock names.
    /// </summary>
    /// <param name="locks">A collection of lock names to release.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task UnlockAll(IEnumerable<string> locks, CancellationToken cancellationToken = default);
}